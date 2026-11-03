namespace HotelAPI.DTOs
{
    public class UserDTO
    {
        public int IdUser { get; set; }
        public string Login { get; set; } = null!;
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public string? UserName { get; set; }
        public RoleSummaryDTO Role { get; set; } = null!;
        public CartSummaryDTO? Cart { get; set; }
        public List<OrderSummaryDTO> Orders { get; set; } = new();
    }
}
