namespace CarSalesManagementAPI.Domain.Entities;

public class Role
{
    public int RoleID { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public int? LastUpdatedBy { get; set; }
    public DateTime? LastUpdatedOn { get; set; }
}
