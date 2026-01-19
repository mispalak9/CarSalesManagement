namespace CarSalesManagementAPI.Domain.Entities;

public class RoleMenuPermission
{
    public int PermissionID { get; set; }
    public int RoleID { get; set; }
    public int MenuID { get; set; }
    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public int? LastUpdatedBy { get; set; }
    public DateTime? LastUpdatedOn { get; set; }
    
    // Navigation properties
    public Role? Role { get; set; }
    public MenuItem? MenuItem { get; set; }
}
