using System.ComponentModel.DataAnnotations;

namespace HotelAPI.DTOs
{
    public class CreateCartDTO
    {
        [Required]
        public int? IdCart { get; set; }
        public int UserId { get; set; }
    }
}
