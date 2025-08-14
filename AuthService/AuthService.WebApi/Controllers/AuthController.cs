using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Application.Interfaces;
using AuthService.Domain.Models;
using AuthService.WebApi.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TaskService.WebApi.Mapping;

namespace AuthService.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;
    private readonly IUserService userService;

    public AuthController(IAuthService authService, IUserService userService)
    {
        this.authService = authService;
        this.userService = userService;
    }
    
    

    [HttpPost("register")]
    [ProducesResponseType(204)]
    public IActionResult Register([FromBody][Required] AuthDataDto authDataDto)
    {
        var user = new NewUser(authDataDto.Login, authDataDto.Password);
        var hashedPassword = authService.GetHashedPassword(user);
        var userWithHashedPassword = new NewUserWithHashedPassword (authDataDto.Login, hashedPassword);
        var idUser = userService.CreateUser(userWithHashedPassword);
        return Ok(new { Token = idUser });
    }
    
    [HttpPost("login")]
    [ProducesResponseType(typeof(object),200)]
    [ProducesResponseType(401)]
    public IActionResult Login([FromBody][Required] AuthDataDto authDataDto)
    {
        UserWithHashedPassword userInDb;
        var isContainsInDb = userService.TryGetUserInDb(authDataDto.Login, out userInDb);
        if (!isContainsInDb)
            return Unauthorized(new 
            {
                error = "Invalid credentials",
                message = "Username or password is incorrect"
            });
        var isValidPassword = authService.IsCorrectPassword(userInDb, authDataDto.Password);
        if (!isValidPassword)
            return Unauthorized(new 
            {
                error = "Invalid credentials",
                message = "Username or password is incorrect"
            });
        var jwtToken = authService.GetJwtTokenWithInfoUser(userInDb.GetInfoAboutMe());
        
        return Ok(new { Token = jwtToken });
    }
    
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfoDto),200)]
    [ProducesResponseType(401)]
    public IActionResult GetInfoAboutMe()
    {
        var jwtToken = HttpContext.Request.Headers["Authorization"]
            .FirstOrDefault()?
            .Split(" ")
            .Last();  

        if (string.IsNullOrEmpty(jwtToken))
        {
            return Unauthorized("JWT token is missing");
        }
        return Ok(authService.GetUserInfoAboutUseByJwt(jwtToken).ToUserInfoDto());
    }
    
    [HttpGet("key")]
    [Produces("application/x-pem-file")]
    public IActionResult GetPublicKey()
    {
        return Content(authService.GetPublicKey(), "application/x-pem-file");
    }
}
