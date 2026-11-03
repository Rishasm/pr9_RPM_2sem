using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace HotelAPI.Swagger;
public class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        if (metadata.OfType<IAllowAnonymous>().Any() || !metadata.OfType<IAuthorizeData>().Any()) return;
        operation.Security = new List<OpenApiSecurityRequirement> {
            new() { [new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            }] = Array.Empty<string>() }
        };
        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Требуется действующий JWT" });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Недостаточно прав" });
    }
}

