using CarSalesManagementAPI.Application.DTOs;

namespace CarSalesManagementAPI.Application.Services;

public interface IMenuService
{
    Task<ApiResponse<UserMenuResponseDto>> GetUserMenusAsync(int userId);
    Task<ApiResponse<IEnumerable<MenuDto>>> GetMenusByRoleIdAsync(int roleId);
}
