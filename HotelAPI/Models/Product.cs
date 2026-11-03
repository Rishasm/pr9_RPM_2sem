using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class Product
{
    public int IdProduct { get; set; }

    public string ProductName { get; set; } = null!;

    public int AvailableQuantity { get; set; }

    public decimal Price { get; set; }

    public int CategoryId { get; set; }

    public string? Description { get; set; }

    public string? ImagePath { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<ProductReservation> ProductReservations { get; set; } = new List<ProductReservation>();
}
