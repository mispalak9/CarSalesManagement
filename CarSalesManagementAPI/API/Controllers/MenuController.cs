using Microsoft.AspNetCore.Mvc;
using CarSalesManagementAPI.Application.DTOs;
using CarSalesManagementAPI.Application.Services;

namespace CarSalesManagementAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;
    private readonly ILogger<MenuController> _logger;

    public MenuController(IMenuService menuService, ILogger<MenuController> logger)
    {
        _menuService = menuService;
        _logger = logger;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<ApiResponse<UserMenuResponseDto>>> GetUserMenus(int userId)
    {
        var response = await _menuService.GetUserMenusAsync(userId);

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

    [HttpGet("role/{roleId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MenuDto>>>> GetMenusByRole(int roleId)
    {
        var response = await _menuService.GetMenusByRoleIdAsync(roleId);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
