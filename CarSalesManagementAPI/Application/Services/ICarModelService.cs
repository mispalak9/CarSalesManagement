using CarSalesManagementAPI.Application.DTOs;

namespace CarSalesManagementAPI.Application.Services;

public interface ICarModelService
{
    Task<ApiResponse<IEnumerable<CarModelDto>>> GetAll(string? searchTerm = null, string? orderBy = null);
    Task<ApiResponse<CarModelDto>> GetById(int id);
    Task<ApiResponse<CarModelDto>> Create(CreateCarModelDto dto);
    Task<ApiResponse<bool>> Update(UpdateCarModelDto dto);
    Task<ApiResponse<bool>> Delete(int id);
    Task<ApiResponse<IEnumerable<CarModelImageDto>>> GetImagesByModelId(int modelId);
    Task<ApiResponse<string>> UploadImage(int modelId, IFormFile file);
    Task<ApiResponse<bool>> SetDefaultImage(int imageId, int modelId);
    Task<ApiResponse<bool>> DeleteImage(int imageId);
    Task<ApiResponse<IEnumerable<BrandDto>>> GetAllBrands();
    Task<ApiResponse<IEnumerable<CarClassDto>>> GetAllClasses();
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
