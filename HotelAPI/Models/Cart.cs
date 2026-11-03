using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class Cart
{
    public int IdCart { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
