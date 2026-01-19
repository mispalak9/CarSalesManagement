namespace CarSalesManagementAPI.Application.DTOs;

public class CreateCarModelDto
{
    public int BrandID { get; set; }
    public int ClassID { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string ModelCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Features { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime DateOfManufacturing { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}
