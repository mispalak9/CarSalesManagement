namespace CarSalesManagementAPI.Domain.Entities;

public class Brand
{
    public int BrandID { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public string BrandCode { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public int? LastUpdatedBy { get; set; }
    public DateTime? LastUpdatedOn { get; set; }
}
