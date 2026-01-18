using CarSalesManagementAPI.Domain.Entities;

namespace CarSalesManagementAPI.Domain.Interfaces;

public interface IMenuRepository
{
    Task<IEnumerable<MenuItem>> GetMenuItemsByRoleIdAsync(int roleId);
    Task<IEnumerable<MenuItem>> GetAllMenuItemsAsync();
    Task<IEnumerable<RoleMenuPermission>> GetRoleMenuPermissionsAsync(int roleId);
    Task<IEnumerable<int>> GetUserRoleIdsAsync(int userId);
}
