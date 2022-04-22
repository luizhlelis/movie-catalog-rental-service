using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using RentMovie.Application.Domain.Dtos;
using RentMovie.Application.Domain.ValueObjects;

namespace RentMovie.Web.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly Authentication _authentication;

    public AuthenticationController(Authentication authentication)
    {
        _authentication = authentication;
    }

    [HttpPost("token")]
    public IActionResult Token([FromBody] OwnerCredentialDto credential)
    {
        var accessToken = _authentication.GenerateAccessToken(credential.Username);
        return Ok(
            new {TokenType = JwtBearerDefaults.AuthenticationScheme, AccessToken = accessToken}
        );
    }
}
