namespace HotelAPI.DTOs
{
    public class PatchUserDTO
    {
        public string? Login { get; set; }
        public string? UserPassword { get; set; }
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
        public string? UserName { get; set; }
    }
}