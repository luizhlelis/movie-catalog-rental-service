using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RentMovie.Application.Domain.Enums;
using RentMovie.Application.Ports;

namespace RentMovie.Web.Filters;

public class AuthorizeOnly : ActionFilterAttribute, IAsyncAuthorizationFilter
{
    private readonly Role _role;

    public AuthorizeOnly(Role role)
    {
        _role = role;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var databaseDrivenPort =
            context.HttpContext.RequestServices.GetRequiredService<IDatabaseDrivenPort>();

        var requesterUsername =
            context.HttpContext.User.FindFirst(
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");

        var user = await databaseDrivenPort.GetUserAsync(requesterUsername?.Value ?? "");

        if (user?.Role != _role)
            context.Result = new UnauthorizedResult();
    }
}
