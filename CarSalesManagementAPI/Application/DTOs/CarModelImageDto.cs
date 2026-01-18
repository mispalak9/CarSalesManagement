namespace CarSalesManagementAPI.Application.DTOs;

public class CarModelImageDto
{
    public int ImageID { get; set; }
    public int ModelID { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string ImageName { get; set; } = string.Empty;
    public long ImageSize { get; set; }
    public bool IsDefault { get; set; }
    public int SortOrder { get; set; }
}
