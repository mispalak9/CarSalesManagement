using CarSalesManagementAPI.Domain.Entities;

namespace CarSalesManagementAPI.Domain.Interfaces;

public interface IMenuRepository
{
    Task<IEnumerable<MenuItem>> GetMenuItemsByRoleId(int roleId);
    Task<IEnumerable<MenuItem>> GetAllMenuItems();
    Task<IEnumerable<RoleMenuPermission>> GetRoleMenuPermissions(int roleId);
    Task<IEnumerable<int>> GetUserRoleIds(int userId);
}
