using CarSalesManagementAPI.Domain.Entities;

namespace CarSalesManagementAPI.Domain.Interfaces;

public interface ICarModelRepository
{
    Task<IEnumerable<CarModel>> GetAllAsync(string? searchTerm = null, string? orderBy = null);
    Task<CarModel?> GetByIdAsync(int id);
    Task<CarModel?> GetByModelCodeAsync(string modelCode);
    Task<int> CreateAsync(CarModel model);
    Task<bool> UpdateAsync(CarModel model);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<CarModelImage>> GetImagesByModelIdAsync(int modelId);
    Task<int> AddImageAsync(CarModelImage image);
    Task<bool> DeleteImageAsync(int imageId);
    Task<bool> SetDefaultImageAsync(int imageId, int modelId);
    Task<Brand?> GetBrandByIdAsync(int brandId);
    Task<CarClass?> GetClassByIdAsync(int classId);
    Task<IEnumerable<Brand>> GetAllBrandsAsync();
    Task<IEnumerable<CarClass>> GetAllClassesAsync();
}
