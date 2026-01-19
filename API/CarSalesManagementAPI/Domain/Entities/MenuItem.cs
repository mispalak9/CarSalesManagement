namespace CarSalesManagementAPI.Domain.Entities;

public class MenuItem
{
    public int MenuID { get; set; }
    public string MenuName { get; set; } = string.Empty;
    public string MenuTitle { get; set; } = string.Empty;
    public string? MenuURL { get; set; }
    public int? ParentMenuID { get; set; }
    public string? IconClass { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public int? LastUpdatedBy { get; set; }
    public DateTime? LastUpdatedOn { get; set; }
    
    // Navigation properties
    public MenuItem? ParentMenu { get; set; }
    public List<MenuItem>? ChildMenus { get; set; }
    public List<RoleMenuPermission>? RolePermissions { get; set; }
}
