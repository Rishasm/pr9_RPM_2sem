using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class Order
{
    public int IdOrder { get; set; }

    public int UserId { get; set; }

    public DateOnly OrderDate { get; set; }

    public virtual User User { get; set; } = null!;
}
