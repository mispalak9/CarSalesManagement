namespace CarSalesManagementAPI.Domain.Entities;

public class CarModelImage
{
    public int ImageID { get; set; }
    public int ModelID { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string ImageName { get; set; } = string.Empty;
    public long ImageSize { get; set; }
    public bool IsDefault { get; set; }
    public int SortOrder { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public int? LastUpdatedBy { get; set; }
    public DateTime? LastUpdatedOn { get; set; }
}
