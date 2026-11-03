using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Models;

public partial class HotelContext : DbContext
{
    public HotelContext()
    {
    }

    public HotelContext(DbContextOptions<HotelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<ProductReservation> ProductReservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.IdCart).HasName("PK__carts__70179490D96D236D");

            entity.ToTable("carts");

            entity.HasIndex(e => e.UserId, "UQ__carts__B9BF3306749A80D5").IsUnique();

            entity.Property(e => e.IdCart)
                .ValueGeneratedNever()
                .HasColumnName("ID_cart");
            entity.Property(e => e.UserId).HasColumnName("user_ID");

            entity.HasOne(d => d.User).WithOne(p => p.Cart)
                .HasForeignKey<Cart>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__carts__user_ID__7A3223E8");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.IdCartItem).HasName("PK__cart_ite__2D21319620DE5FD8");

            entity.ToTable("cart_items");

            entity.Property(e => e.IdCartItem).HasColumnName("ID_cart_item");
            entity.Property(e => e.CartId).HasColumnName("cart_ID");
            entity.Property(e => e.ProductId).HasColumnName("product_ID");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.IdCategory).HasName("PK__categori__AC5CA40EF6E0BDF2");

            entity.ToTable("categories");

            entity.HasIndex(e => e.CategoryName, "UQ__categori__5189E25571480641").IsUnique();

            entity.Property(e => e.IdCategory).HasColumnName("ID_category");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("category_name");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.IdOrder).HasName("PK__orders__89BFCD5913EC4DDE");

            entity.ToTable("orders");

            entity.Property(e => e.IdOrder).HasColumnName("ID_order");
            entity.Property(e => e.OrderDate).HasColumnName("order_date");
            entity.Property(e => e.UserId).HasColumnName("user_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__orders__user_ID__3F115E1A");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.IdOrderItem).HasName("PK__order_it__5B1127FF7FA108B8");

            entity.ToTable("order_items");

            entity.Property(e => e.IdOrderItem)
                .ValueGeneratedNever()
                .HasColumnName("ID_order_item");
            entity.Property(e => e.OrderId).HasColumnName("order_ID");
            entity.Property(e => e.ProductId).HasColumnName("product_ID");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__order_ite__order__03BB8E22");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__order_ite__produ__04AFB25B");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct).HasName("PK__products__FD7FEC2304C0FEDE");

            entity.ToTable("products");

            entity.Property(e => e.IdProduct).HasColumnName("ID_product");
            entity.Property(e => e.CategoryId).HasColumnName("category_ID");
            entity.Property(e => e.AvailableQuantity)
                .HasColumnName("available_quantity")
                .HasDefaultValue(0);
            entity.ToTable("products", table =>
                table.HasCheckConstraint("CK_products_available_quantity", "[available_quantity] >= 0"));
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ImagePath)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("image_path");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("product_name");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__products__catego__339FAB6E");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("PK__roles__45DFFBDBE73A99E3");

            entity.ToTable("roles");

            entity.HasIndex(e => e.RoleName, "UQ__roles__783254B185CBBB04").IsUnique();

            entity.Property(e => e.IdRole).HasColumnName("ID_role");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PK__users__D7B4671EE4DBB312");

            entity.ToTable("users");

            entity.HasIndex(e => e.Login, "UQ__users__7838F272C1F05996").IsUnique();

            entity.Property(e => e.IdUser).HasColumnName("ID_user");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("login");
            entity.Property(e => e.RoleId).HasColumnName("role_ID");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("user_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("password_hash");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__users__role_ID__2CF2ADDF");
        });

        modelBuilder.Entity<ProductReservation>(entity =>
        {
            entity.ToTable("product_reservations", table =>
                table.HasCheckConstraint("CK_product_reservations_quantity", "[quantity] > 0"));
            entity.HasKey(e => e.IdProductReservation).HasName("PK_product_reservations");
            entity.Property(e => e.IdProductReservation).HasColumnName("ID_product_reservation").UseIdentityColumn();
            entity.Property(e => e.ProductId).HasColumnName("product_ID");
            entity.Property(e => e.UserId).HasColumnName("user_ID");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at").HasColumnType("datetime2(7)");
            entity.HasIndex(e => e.ProductId).HasDatabaseName("IX_product_reservations_product_ID");
            entity.HasIndex(e => e.UserId).HasDatabaseName("IX_product_reservations_user_ID");
            entity.HasOne(e => e.Product).WithMany(p => p.ProductReservations)
                .HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_product_reservations_products");
            entity.HasOne(e => e.User).WithMany(u => u.ProductReservations)
                .HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_product_reservations_users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
