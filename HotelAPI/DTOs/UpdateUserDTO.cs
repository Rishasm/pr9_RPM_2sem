namespace HotelAPI.DTOs
{
    public class UpdateUserDTO
    {
        public string Login { get; set; } = null!;
        public string UserPassword { get; set; } = null!;
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public string? UserName { get; set; }
    }
}