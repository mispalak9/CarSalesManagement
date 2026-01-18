namespace CarSalesManagementAPI.Application.DTOs;

public class MenuDto
{
    public int MenuID { get; set; }
    public string MenuName { get; set; } = string.Empty;
    public string MenuTitle { get; set; } = string.Empty;
    public string? MenuURL { get; set; }
    public int? ParentMenuID { get; set; }
    public string? IconClass { get; set; }
    public int SortOrder { get; set; }
    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public List<MenuDto>? ChildMenus { get; set; }
}

public class UserMenuResponseDto
{
    public int UserID { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public List<MenuDto> Menus { get; set; } = new();
}
