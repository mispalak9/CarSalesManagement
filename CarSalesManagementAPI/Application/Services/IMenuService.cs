using CarSalesManagementAPI.Application.DTOs;

namespace CarSalesManagementAPI.Application.Services;

public interface IMenuService
{
    Task<ApiResponse<UserMenuResponseDto>> GetUserMenus(int userId);
    Task<ApiResponse<IEnumerable<MenuDto>>> GetMenusByRoleId(int roleId);
}
