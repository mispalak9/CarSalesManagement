using CarSalesManagementAPI.Domain.Entities;

namespace CarSalesManagementAPI.Domain.Interfaces;

public interface ICarModelRepository
{
    Task<IEnumerable<CarModel>> GetAll(string? searchTerm = null, string? orderBy = null);
    Task<CarModel?> GetById(int id);
    Task<CarModel?> GetByModelCode(string modelCode);
    Task<int> Create(CarModel model);
    Task<bool> Update(CarModel model);
    Task<bool> Delete(int id);
    Task<IEnumerable<CarModelImage>> GetImagesByModelId(int modelId);
    Task<IEnumerable<CarModelImage>> GetImagesByModelIds(IEnumerable<int> modelIds);
    Task<int> AddImage(CarModelImage image);
    Task<bool> SetDefaultImage(int imageId, int modelId);
    Task<bool> DeleteImage(int imageId);
    Task<IEnumerable<Brand>> GetAllBrands();
    Task<Brand?> GetBrandById(int brandId);
    Task<IEnumerable<CarClass>> GetAllClasses();
    Task<CarClass?> GetClassById(int classId);
}
