using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelAPI.Models;

public partial class CartItem
{
    public int IdCartItem { get; set; }

    public int CartId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    // Related records are loaded by the repository; no new columns or database FKs.
    [NotMapped]
    public Cart? Cart { get; set; }

    [NotMapped]
    public Product? Product { get; set; }
}
