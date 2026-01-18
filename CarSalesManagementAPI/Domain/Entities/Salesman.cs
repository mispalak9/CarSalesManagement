namespace CarSalesManagementAPI.Domain.Entities;

public class Salesman
{
    public int SalesmanID { get; set; }
    public string SalesmanCode { get; set; } = string.Empty;
    public string SalesmanName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public int? LastUpdatedBy { get; set; }
    public DateTime? LastUpdatedOn { get; set; }
}
