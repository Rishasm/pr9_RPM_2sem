using System.Security.Claims;
using HotelAPI.Models;
using Microsoft.AspNetCore.Authorization;
namespace HotelAPI.Authorization;
public sealed class ReservationOwnerOrAdminRequirement : IAuthorizationRequirement { }
public sealed class ReservationOwnerOrAdminHandler :
    AuthorizationHandler<ReservationOwnerOrAdminRequirement, ProductReservation>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        ReservationOwnerOrAdminRequirement requirement, ProductReservation resource)
    {
        if (context.User.Identity?.IsAuthenticated == true &&
            (context.User.IsInRole("Admin") ||
             (int.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) && id == resource.UserId)))
            context.Succeed(requirement);
        return Task.CompletedTask;
    }
}

