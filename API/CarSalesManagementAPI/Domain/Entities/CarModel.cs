namespace CarSalesManagementAPI.Domain.Entities;

public class CarModel
{
    public int ModelID { get; set; }
    public int BrandID { get; set; }
    public int ClassID { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string ModelCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Features { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime DateOfManufacturing { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public int? LastUpdatedBy { get; set; }
    public DateTime? LastUpdatedOn { get; set; }
    
    // Navigation properties
    public Brand? Brand { get; set; }
    public CarClass? CarClass { get; set; }
    public List<CarModelImage>? Images { get; set; }
}
