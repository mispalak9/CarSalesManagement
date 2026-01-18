using CarSalesManagementAPI.Application.DTOs;

namespace CarSalesManagementAPI.Application.Services;

public interface ICarModelService
{
    Task<ApiResponse<IEnumerable<CarModelDto>>> GetAllAsync(string? searchTerm = null, string? orderBy = null);
    Task<ApiResponse<CarModelDto>> GetByIdAsync(int id);
    Task<ApiResponse<CarModelDto>> CreateAsync(CreateCarModelDto dto);
    Task<ApiResponse<bool>> UpdateAsync(UpdateCarModelDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
    Task<ApiResponse<IEnumerable<CarModelImageDto>>> GetImagesByModelIdAsync(int modelId);
    Task<ApiResponse<string>> UploadImageAsync(int modelId, IFormFile file);
    Task<ApiResponse<bool>> SetDefaultImageAsync(int imageId, int modelId);
    Task<ApiResponse<bool>> DeleteImageAsync(int imageId);
    Task<ApiResponse<IEnumerable<BrandDto>>> GetAllBrandsAsync();
    Task<ApiResponse<IEnumerable<CarClassDto>>> GetAllClassesAsync();
}

public class BrandDto
{
    public int BrandID { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public string BrandCode { get; set; } = string.Empty;
}

public class CarClassDto
{
    public int ClassID { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string ClassCode { get; set; } = string.Empty;
}
