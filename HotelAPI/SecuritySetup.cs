using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using HotelAPI.Authorization;
using HotelAPI.Repositories;
using HotelAPI.Services;
using HotelAPI.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
namespace HotelAPI;
public static class SecuritySetup
{
    public static void AddPracticeSecurity(this WebApplicationBuilder builder)
    {
        var jwt = builder.Configuration.GetSection("Jwt");
        if (Encoding.UTF8.GetByteCount(jwt["Key"] ?? "") < 32)
            throw new InvalidOperationException("Jwt:Key должен содержать минимум 32 байта.");
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IAuthorizationHandler, ReservationOwnerOrAdminHandler>();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true,
                ValidateIssuerSigningKey = true, RequireExpirationTime = true,
                ValidIssuer = jwt["Issuer"], ValidAudience = jwt["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
                ClockSkew = TimeSpan.Zero, ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }
            };
            options.Events = new JwtBearerEvents {
                OnTokenValidated = async context => {
                    if (!int.TryParse(context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
                    { context.Fail("Отсутствует идентификатор пользователя."); return; }
                    var repo = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                    var user = await repo.GetByIdAsync(id, context.HttpContext.RequestAborted);
                    if (user is null || !user.IsActive || user.Role.RoleName != context.Principal?.FindFirstValue(ClaimTypes.Role))
                        context.Fail("Учётная запись отключена или права изменились.");
                }
            };
        });
        builder.Services.AddAuthorization(options => {
            options.AddPolicy("AdminOnly", policy => policy.RequireAuthenticatedUser().RequireRole("Admin"));
            options.AddPolicy("ReservationOwnerOrAdmin", policy =>
                policy.RequireAuthenticatedUser().AddRequirements(new ReservationOwnerOrAdminRequirement()));
        });
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        builder.Services.AddCors(options => options.AddPolicy("FrontendPolicy", policy =>
            policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("X-Total-Count", "Retry-After")));
        builder.Services.AddRateLimiter(options => {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy("LoginPolicy", context =>
                RateLimitPartition.GetFixedWindowLimiter(context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions {
                        Window = TimeSpan.FromMinutes(1), PermitLimit = 5, QueueLimit = 0, AutoReplenishment = true
                    }));
            options.OnRejected = async (context, ct) => {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retry))
                    context.HttpContext.Response.Headers.RetryAfter = Math.Ceiling(retry.TotalSeconds).ToString();
                await context.HttpContext.Response.WriteAsJsonAsync(new { error = "Слишком много попыток входа. Подождите минуту." }, ct);
            };
        });
        builder.Services.AddSwaggerGen(options => {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT",
                Description = "Введите JWT без префикса Bearer."
            });
            options.OperationFilter<AuthorizeOperationFilter>();
        });
    }
}

