namespace CarSalesManagementAPI.Application.DTOs;

public class CarModelDto
{
    public int ModelID { get; set; }
    public int BrandID { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public int ClassID { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public string ModelCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Features { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime DateOfManufacturing { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public List<CarModelImageDto>? Images { get; set; }
}
