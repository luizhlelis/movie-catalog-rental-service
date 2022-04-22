using Microsoft.AspNetCore.Mvc;
using RentMovie.Application.Dtos;
using RentMovie.Application.Domain.Entities;

namespace RentMovie.Web.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class UserController : ControllerBase
{
    [HttpPost]
    public IActionResult Post(UserDto user)
    {
        return Ok();
    }
}
