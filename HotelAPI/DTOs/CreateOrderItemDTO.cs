using System.ComponentModel.DataAnnotations;

namespace HotelAPI.DTOs
{
    public class CreateOrderItemDTO
    {
        [Required]
        public int? IdOrderItem { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
