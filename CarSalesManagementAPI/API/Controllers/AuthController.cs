using Microsoft.AspNetCore.Mvc;
using CarSalesManagementAPI.Application.DTOs;
using CarSalesManagementAPI.Application.Services;

namespace CarSalesManagementAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginDto loginDto)
    {
        var response = await _authService.LoginAsync(loginDto);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> GetUserInfo(int userId)
    {
        var response = await _authService.GetUserInfoAsync(userId);

        if (!response.Success)
        {
            if (response.Errors.Any(e => e.Contains("not found")))
            {
                return NotFound(response);
            }
            return BadRequest(response);
        }

        return Ok(response);
    }
}
