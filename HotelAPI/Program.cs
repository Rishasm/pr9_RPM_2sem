using HotelAPI;
using HotelAPI.Data;
using HotelAPI.Mapping;
using HotelAPI.Repositories;
using HotelAPI.Services;

using HotelAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HotelContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<ICartItemService, CartItemService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IProductReservationRepository, ProductReservationRepository>();
builder.Services.AddScoped<IProductReservationService, ProductReservationService>();
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.AddPracticeSecurity();

var app = builder.Build();
if (args.Contains("--initialize-security"))
{
    using var scope = app.Services.CreateScope();
    await DatabaseInitializer.InitializeAsync(scope.ServiceProvider.GetRequiredService<HotelContext>());
    return;
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("FrontendPolicy");
app.UseStatusCodePages(async context => {
    var response = context.HttpContext.Response;
    var message = response.StatusCode switch {
        401 => "Требуется вход с действующим JWT.",
        403 => "Недостаточно прав для этого действия.",
        404 => "Ресурс не найден.",
        _ => "Запрос не выполнен."
    };
    await response.WriteAsJsonAsync(new { error = message, status = response.StatusCode });
});
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();

app.Run();

