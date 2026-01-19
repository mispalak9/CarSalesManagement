using Dapper;
using CarSalesManagementAPI.Domain.Entities;
using CarSalesManagementAPI.Domain.Interfaces;
using CarSalesManagementAPI.Infrastructure.Data;
using System.Data;

namespace CarSalesManagementAPI.Infrastructure.Repositories;

public class MenuRepository : IMenuRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public MenuRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<MenuItem>> GetMenuItemsByRoleId(int roleId)
    {
        using var connection = _connectionFactory.CreateConnection();

        // Get menu items that the role has permission to view
        var sql = @"SELECT DISTINCT m.* 
                    FROM MenuItems m
                    INNER JOIN RoleMenuPermissions rmp ON m.MenuID = rmp.MenuID
                    WHERE rmp.RoleID = @RoleId 
                    AND m.IsActive = 1 
                    AND rmp.CanView = 1
                    ORDER BY m.SortOrder ASC, m.MenuID ASC";

        var menuItems = await connection.QueryAsync<MenuItem>(sql, new { RoleId = roleId });

        // Build hierarchical menu structure
        var menuList = menuItems.ToList();
        var parentMenus = menuList.Where(m => m.ParentMenuID == null).OrderBy(m => m.SortOrder).ToList();

        foreach (var parent in parentMenus)
        {
            parent.ChildMenus = menuList
                .Where(m => m.ParentMenuID == parent.MenuID)
                .OrderBy(m => m.SortOrder)
                .ToList();
        }

        return parentMenus;
    }

    public async Task<IEnumerable<MenuItem>> GetAllMenuItems()
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"SELECT * FROM MenuItems 
                    WHERE IsActive = 1 
                    ORDER BY SortOrder ASC, MenuID ASC";

        var menuItems = await connection.QueryAsync<MenuItem>(sql);
        var menuList = menuItems.ToList();

        // Build hierarchical structure
        var parentMenus = menuList.Where(m => m.ParentMenuID == null).OrderBy(m => m.SortOrder).ToList();

        foreach (var parent in parentMenus)
        {
            parent.ChildMenus = menuList
                .Where(m => m.ParentMenuID == parent.MenuID)
                .OrderBy(m => m.SortOrder)
                .ToList();
        }

        return parentMenus;
    }

    public async Task<IEnumerable<RoleMenuPermission>> GetRoleMenuPermissions(int roleId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"SELECT * FROM RoleMenuPermissions 
                    WHERE RoleID = @RoleId
                    ORDER BY MenuID";

        return await connection.QueryAsync<RoleMenuPermission>(sql, new { RoleId = roleId });
    }

    public async Task<IEnumerable<int>> GetUserRoleIds(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
        SELECT RoleID
        FROM dbo.UserRoles
        WHERE UserID = @UserId";

        return await connection.QueryAsync<int>(sql, new { UserId = userId });
    }
}