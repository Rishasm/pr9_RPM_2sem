using HotelAPI.DTOs;

namespace HotelAPI.Services;

public interface IProductReservationService
{
    Task<HotelAPI.Models.ProductReservation?> GetResourceAsync(int id, CancellationToken ct);
    Task<(List<ProductReservationDTO> Items, int TotalCount)> GetVisibleAsync(int page, int pageSize, int? userId, CancellationToken ct);
    Task<(List<ProductReservationDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<ProductReservationDTO?> GetByIdAsync(int id, CancellationToken ct);
    Task<ProductReservationDTO?> ReserveAsync(CreateProductReservationDTO dto, CancellationToken ct);
    Task<bool> CancelAsync(int id, CancellationToken ct);
}
