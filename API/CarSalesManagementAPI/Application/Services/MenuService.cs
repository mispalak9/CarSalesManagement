using AutoMapper;
using CarSalesManagementAPI.Application.DTOs;
using CarSalesManagementAPI.Domain.Entities;
using CarSalesManagementAPI.Domain.Interfaces;

namespace CarSalesManagementAPI.Application.Services;

public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;

    public MenuService(IMenuRepository menuRepository, IAuthRepository authRepository, IMapper mapper)
    {
        _menuRepository = menuRepository;
        _authRepository = authRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<UserMenuResponseDto>> GetUserMenus(int userId)
    {
        try
        {
            var user = await _authRepository.GetUserById(userId);
            if (user == null)
            {
                return new ApiResponse<UserMenuResponseDto>
                {
                    Success = false,
                    Message = "User not found.",
                    Errors = new List<string> { $"User with ID {userId} not found." }
                };
            }

            // Get user roles
            var roleIds = await _menuRepository.GetUserRoleIds(userId);

            if (!roleIds.Any())
            {
                return new ApiResponse<UserMenuResponseDto>
                {
                    Success = true,
                    Message = "User has no roles assigned.",
                    Data = new UserMenuResponseDto
                    {
                        UserID = user.UserID,
                        Username = user.Username,
                        FullName = user.FullName,
                        Roles = new List<string>(),
                        Menus = new List<MenuDto>()
                    }
                };
            }

            // Get all menus for user's roles and merge permissions
            var allMenuItems = new List<MenuItem>();
            var allPermissions = new Dictionary<int, RoleMenuPermission>();

            foreach (var roleId in roleIds)
            {
                var menuItems = await _menuRepository.GetMenuItemsByRoleId(roleId);
                allMenuItems.AddRange(menuItems);

                var permissions = await _menuRepository.GetRoleMenuPermissions(roleId);
                foreach (var perm in permissions)
                {
                    if (!allPermissions.ContainsKey(perm.MenuID))
                    {
                        allPermissions[perm.MenuID] = perm;
                    }
                    else
                    {
                        // Merge permissions (if any role has permission, user has it)
                        var existing = allPermissions[perm.MenuID];
                        existing.CanView = existing.CanView || perm.CanView;
                        existing.CanCreate = existing.CanCreate || perm.CanCreate;
                        existing.CanEdit = existing.CanEdit || perm.CanEdit;
                        existing.CanDelete = existing.CanDelete || perm.CanDelete;
                    }
                }
            }

            // Remove duplicates and build menu DTOs
            var uniqueMenus = allMenuItems
                .GroupBy(m => m.MenuID)
                .Select(g => g.First())
                .OrderBy(m => m.SortOrder)
                .ToList();

            var menuDtos = MapMenusWithPermissions(uniqueMenus, allPermissions);

            var response = new UserMenuResponseDto
            {
                UserID = user.UserID,
                Username = user.Username,
                FullName = user.FullName,
                Roles = new List<string>(), // TODO: Get user roles from auth repository
                Menus = menuDtos
            };

            return new ApiResponse<UserMenuResponseDto>
            {
                Success = true,
                Message = "User menus retrieved successfully.",
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<UserMenuResponseDto>
            {
                Success = false,
                Message = "Error retrieving user menus.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<IEnumerable<MenuDto>>> GetMenusByRoleId(int roleId)
    {
        try
        {
            var menuItems = await _menuRepository.GetMenuItemsByRoleId(roleId);
            var permissions = await _menuRepository.GetRoleMenuPermissions(roleId);
            var permissionsDict = permissions.ToDictionary(p => p.MenuID);

            var menuDtos = MapMenusWithPermissions(menuItems, permissionsDict);

            return new ApiResponse<IEnumerable<MenuDto>>
            {
                Success = true,
                Message = "Role menus retrieved successfully.",
                Data = menuDtos
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<MenuDto>>
            {
                Success = false,
                Message = "Error retrieving role menus.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    private List<MenuDto> MapMenusWithPermissions(IEnumerable<MenuItem> menuItems, Dictionary<int, RoleMenuPermission> permissions)
    {
        var menuDtos = new List<MenuDto>();

        foreach (var menu in menuItems.Where(m => m.ParentMenuID == null).OrderBy(m => m.SortOrder))
        {
            var menuDto = new MenuDto
            {
                MenuID = menu.MenuID,
                MenuName = menu.MenuName,
                MenuTitle = menu.MenuTitle,
                MenuURL = menu.MenuURL,
                ParentMenuID = menu.ParentMenuID,
                IconClass = menu.IconClass,
                SortOrder = menu.SortOrder
            };

            // Set permissions if available
            if (permissions.TryGetValue(menu.MenuID, out var permission))
            {
                menuDto.CanView = permission.CanView;
                menuDto.CanCreate = permission.CanCreate;
                menuDto.CanEdit = permission.CanEdit;
                menuDto.CanDelete = permission.CanDelete;
            }
            else
            {
                menuDto.CanView = true; // Default to view only
            }

            // Map child menus
            if (menu.ChildMenus != null && menu.ChildMenus.Any())
            {
                menuDto.ChildMenus = menu.ChildMenus
                    .OrderBy(m => m.SortOrder)
                    .Select(child =>
                    {
                        var childDto = new MenuDto
                        {
                            MenuID = child.MenuID,
                            MenuName = child.MenuName,
                            MenuTitle = child.MenuTitle,
                            MenuURL = child.MenuURL,
                            ParentMenuID = child.ParentMenuID,
                            IconClass = child.IconClass,
                            SortOrder = child.SortOrder
                        };

                        if (permissions.TryGetValue(child.MenuID, out var childPermission))
                        {
                            childDto.CanView = childPermission.CanView;
                            childDto.CanCreate = childPermission.CanCreate;
                            childDto.CanEdit = childPermission.CanEdit;
                            childDto.CanDelete = childPermission.CanDelete;
                        }
                        else
                        {
                            childDto.CanView = true;
                        }

                        return childDto;
                    })
                    .ToList();
            }

            menuDtos.Add(menuDto);
        }

        return menuDtos;
    }
}
