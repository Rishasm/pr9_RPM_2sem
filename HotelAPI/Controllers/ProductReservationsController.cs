using System.Security.Claims;
using HotelAPI.DTOs;
using HotelAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HotelAPI.Controllers;
[ApiController, Authorize, Route("api/productreservations")]
public class ProductReservationsController(IProductReservationService service, IAuthorizationService authorization) : ControllerBase
{
    private int CurrentId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        try {
            var (items, total) = await service.GetVisibleAsync(page, pageSize, User.IsInRole("Admin") ? null : CurrentId, ct);
            Response.Headers["X-Total-Count"] = total.ToString();
            return Ok(items);
        } catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var resource = await service.GetResourceAsync(id, ct);
        if (resource is null) return NotFound();
        if (!(await authorization.AuthorizeAsync(User, resource, "ReservationOwnerOrAdmin")).Succeeded) return Forbid();
        return Ok(await service.GetByIdAsync(id, ct));
    }
    [HttpPost]
    public async Task<IActionResult> Reserve(CreateProductReservationDTO dto, CancellationToken ct)
    {
        try {
            dto.UserId = CurrentId;
            var created = await service.ReserveAsync(dto, ct);
            return created is null ? NotFound() : CreatedAtAction(nameof(GetById), new { id = created.IdProductReservation }, created);
        } catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }
    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        var resource = await service.GetResourceAsync(id, ct);
        if (resource is null) return NotFound();
        if (!(await authorization.AuthorizeAsync(User, resource, "ReservationOwnerOrAdmin")).Succeeded) return Forbid();
        if (resource.CancelledAt is not null) return Conflict(new { error = "Бронирование уже отменено." });
        try {
            return await service.CancelAsync(id, ct) ? NoContent() : NotFound();
        } catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }
}

