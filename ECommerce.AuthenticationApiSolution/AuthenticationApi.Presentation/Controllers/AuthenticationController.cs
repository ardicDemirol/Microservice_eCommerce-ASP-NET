using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using ECommerce.SharedLibrary.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Presentation.Controllers;
[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthenticationController(IUser user) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<Response>> Login(LoginDto loginDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await user.Login(loginDto);
        return result.Flag ? Ok(result) : BadRequest(result);
    }

    [HttpPost("register")]
    public async Task<ActionResult<Response>> Register(AppUserDto appUserDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await user.Register(appUserDto);
        return result.Flag ? Ok(result) : BadRequest(result);
    }

    [HttpGet("getUser/{userId:int}")]
    [Authorize]
    public async Task<ActionResult<GetUserDto>> GetUser(int userId)
    {
        var currentUser = await user.GetUser(userId);
        return currentUser.Id > 0 ? Ok(currentUser) : BadRequest(currentUser);

    }

}
