using AutoMapper;
using HotelAPI.DTOs;
using HotelAPI.Models;

namespace HotelAPI.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDTO>();
        CreateMap<Category, CategoryDTO>();
        CreateMap<User, UserDTO>();
        CreateMap<Role, RoleDTO>();
        CreateMap<Cart, CartDTO>();
        CreateMap<CartItem, CartItemDTO>();
        CreateMap<Order, OrderDTO>();
        CreateMap<OrderItem, OrderItemDTO>();
        CreateMap<Product, ProductSummaryDTO>();
        CreateMap<Category, CategorySummaryDTO>();
        CreateMap<User, UserSummaryDTO>();
        CreateMap<Role, RoleSummaryDTO>();
        CreateMap<Cart, CartSummaryDTO>();
        CreateMap<Order, OrderSummaryDTO>();
        CreateMap<ProductReservation, ProductReservationDTO>();
        CreateMap<CreateProductDTO, Product>()
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.OrderItems, o => o.Ignore())
            .ForMember(d => d.ProductReservations, o => o.Ignore())
            .ForMember(d => d.IdProduct, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());
        CreateMap<UpdateProductDTO, Product>()
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.OrderItems, o => o.Ignore())
            .ForMember(d => d.ProductReservations, o => o.Ignore())
            .ForMember(d => d.IdProduct, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());
        CreateMap<PatchProductDTO, Product>()
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.OrderItems, o => o.Ignore())
            .ForMember(d => d.ProductReservations, o => o.Ignore())
            .ForMember(d => d.IdProduct, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.ProductName, o => o.PreCondition(s => s.ProductName != null))
            .ForMember(d => d.Description, o => o.PreCondition(s => s.Description != null))
            .ForMember(d => d.Price, o => o.PreCondition(s => s.Price != null))
            .ForMember(d => d.CategoryId, o => o.PreCondition(s => s.CategoryId != null))
            .ForMember(d => d.ImagePath, o => o.PreCondition(s => s.ImagePath != null))
            .ForMember(d => d.AvailableQuantity, o => o.PreCondition(s => s.AvailableQuantity != null));
        CreateMap<CreateCategoryDTO, Category>()
            .ForMember(d => d.Products, o => o.Ignore())
            .ForMember(d => d.IdCategory, o => o.Ignore());
        CreateMap<UpdateCategoryDTO, Category>()
            .ForMember(d => d.Products, o => o.Ignore())
            .ForMember(d => d.IdCategory, o => o.Ignore());
        CreateMap<PatchCategoryDTO, Category>()
            .ForMember(d => d.Products, o => o.Ignore())
            .ForMember(d => d.IdCategory, o => o.Ignore())
            .ForMember(d => d.CategoryName, o => o.PreCondition(s => s.CategoryName != null));
        CreateMap<CreateUserDTO, User>()
            .ForMember(d => d.PasswordHash, o => o.Ignore())
            .ForMember(d => d.Role, o => o.Ignore())
            .ForMember(d => d.Cart, o => o.Ignore())
            .ForMember(d => d.Orders, o => o.Ignore())
            .ForMember(d => d.ProductReservations, o => o.Ignore())
            .ForMember(d => d.IdUser, o => o.Ignore());
        CreateMap<UpdateUserDTO, User>()
            .ForMember(d => d.PasswordHash, o => o.Ignore())
            .ForMember(d => d.Role, o => o.Ignore())
            .ForMember(d => d.Cart, o => o.Ignore())
            .ForMember(d => d.Orders, o => o.Ignore())
            .ForMember(d => d.ProductReservations, o => o.Ignore())
            .ForMember(d => d.IdUser, o => o.Ignore());
        CreateMap<PatchUserDTO, User>()
            .ForMember(d => d.Role, o => o.Ignore())
            .ForMember(d => d.Cart, o => o.Ignore())
            .ForMember(d => d.Orders, o => o.Ignore())
            .ForMember(d => d.ProductReservations, o => o.Ignore())
            .ForMember(d => d.IdUser, o => o.Ignore())
            .ForMember(d => d.Login, o => o.PreCondition(s => s.Login != null))
            .ForMember(d => d.PasswordHash, o => o.Ignore())
            .ForMember(d => d.RoleId, o => o.PreCondition(s => s.RoleId != null))
            .ForMember(d => d.IsActive, o => o.PreCondition(s => s.IsActive != null))
            .ForMember(d => d.UserName, o => o.PreCondition(s => s.UserName != null));
        CreateMap<CreateRoleDTO, Role>()
            .ForMember(d => d.Users, o => o.Ignore())
            .ForMember(d => d.IdRole, o => o.Ignore());
        CreateMap<UpdateRoleDTO, Role>()
            .ForMember(d => d.Users, o => o.Ignore())
            .ForMember(d => d.IdRole, o => o.Ignore());
        CreateMap<PatchRoleDTO, Role>()
            .ForMember(d => d.Users, o => o.Ignore())
            .ForMember(d => d.IdRole, o => o.Ignore())
            .ForMember(d => d.RoleName, o => o.PreCondition(s => s.RoleName != null));
        CreateMap<CreateCartDTO, Cart>()
            .ForMember(d => d.User, o => o.Ignore());
        CreateMap<UpdateCartDTO, Cart>()
            .ForMember(d => d.User, o => o.Ignore())
            .ForMember(d => d.IdCart, o => o.Ignore());
        CreateMap<PatchCartDTO, Cart>()
            .ForMember(d => d.User, o => o.Ignore())
            .ForMember(d => d.IdCart, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.PreCondition(s => s.UserId != null));
        CreateMap<CreateCartItemDTO, CartItem>()
            .ForMember(d => d.Cart, o => o.Ignore())
            .ForMember(d => d.Product, o => o.Ignore())
            .ForMember(d => d.IdCartItem, o => o.Ignore());
        CreateMap<UpdateCartItemDTO, CartItem>()
            .ForMember(d => d.Cart, o => o.Ignore())
            .ForMember(d => d.Product, o => o.Ignore())
            .ForMember(d => d.IdCartItem, o => o.Ignore());
        CreateMap<PatchCartItemDTO, CartItem>()
            .ForMember(d => d.Cart, o => o.Ignore())
            .ForMember(d => d.Product, o => o.Ignore())
            .ForMember(d => d.IdCartItem, o => o.Ignore())
            .ForMember(d => d.CartId, o => o.PreCondition(s => s.CartId != null))
            .ForMember(d => d.ProductId, o => o.PreCondition(s => s.ProductId != null))
            .ForMember(d => d.Quantity, o => o.PreCondition(s => s.Quantity != null));
        CreateMap<CreateOrderDTO, Order>()
            .ForMember(d => d.User, o => o.Ignore())
            .ForMember(d => d.OrderItems, o => o.Ignore())
            .ForMember(d => d.IdOrder, o => o.Ignore());
        CreateMap<UpdateOrderDTO, Order>()
            .ForMember(d => d.User, o => o.Ignore())
            .ForMember(d => d.OrderItems, o => o.Ignore())
            .ForMember(d => d.IdOrder, o => o.Ignore());
        CreateMap<PatchOrderDTO, Order>()
            .ForMember(d => d.User, o => o.Ignore())
            .ForMember(d => d.OrderItems, o => o.Ignore())
            .ForMember(d => d.IdOrder, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.PreCondition(s => s.UserId != null))
            .ForMember(d => d.OrderDate, o => o.PreCondition(s => s.OrderDate != null));
        CreateMap<CreateOrderItemDTO, OrderItem>()
            .ForMember(d => d.Order, o => o.Ignore())
            .ForMember(d => d.Product, o => o.Ignore());
        CreateMap<UpdateOrderItemDTO, OrderItem>()
            .ForMember(d => d.Order, o => o.Ignore())
            .ForMember(d => d.Product, o => o.Ignore())
            .ForMember(d => d.IdOrderItem, o => o.Ignore());
        CreateMap<PatchOrderItemDTO, OrderItem>()
            .ForMember(d => d.Order, o => o.Ignore())
            .ForMember(d => d.Product, o => o.Ignore())
            .ForMember(d => d.IdOrderItem, o => o.Ignore())
            .ForMember(d => d.OrderId, o => o.PreCondition(s => s.OrderId != null))
            .ForMember(d => d.ProductId, o => o.PreCondition(s => s.ProductId != null))
            .ForMember(d => d.Quantity, o => o.PreCondition(s => s.Quantity != null));
        CreateMap<CreateProductReservationDTO, ProductReservation>()
            .ForMember(d => d.IdProductReservation, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.CancelledAt, o => o.Ignore())
            .ForMember(d => d.Product, o => o.Ignore())
            .ForMember(d => d.User, o => o.Ignore());
    }
}
