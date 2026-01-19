namespace CarSalesManagementAPI.Domain.Entities;

public class UserRole
{
    public int UserRoleID { get; set; }
    public int UserID { get; set; }
    public int RoleID { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    
    // Navigation properties
    public User? User { get; set; }
    public Role? Role { get; set; }
}
