using Dapper;
using CarSalesManagementAPI.Domain.Entities;
using CarSalesManagementAPI.Domain.Interfaces;
using CarSalesManagementAPI.Infrastructure.Data;
using System.Data;

namespace CarSalesManagementAPI.Infrastructure.Repositories;

public class CarModelRepository : ICarModelRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CarModelRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<CarModel>> GetAllAsync(string? searchTerm = null, string? orderBy = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"SELECT cm.*, b.BrandName, b.BrandCode, cc.ClassName, cc.ClassCode 
                    FROM CarModels cm
                    INNER JOIN Brands b ON cm.BrandID = b.BrandID
                    INNER JOIN CarClasses cc ON cm.ClassID = cc.ClassID
                    WHERE cm.IsActive = 1";

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            sql += " AND (cm.ModelName LIKE @SearchTerm OR cm.ModelCode LIKE @SearchTerm)";
        }

        sql += orderBy?.ToLower() switch
        {
            "date" => " ORDER BY cm.DateOfManufacturing DESC",
            "sortorder" => " ORDER BY cm.SortOrder ASC",
            _ => " ORDER BY cm.DateOfManufacturing DESC, cm.SortOrder ASC"
        };

        return await connection.QueryAsync<CarModel, Brand, CarClass, CarModel>(
            sql,
            (model, brand, carClass) =>
            {
                model.Brand = brand;
                model.CarClass = carClass;
                return model;
            },
            new { SearchTerm = $"%{searchTerm}%" },
            splitOn: "BrandID,ClassID"
        );
    }

    public async Task<CarModel?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"SELECT cm.*, b.BrandName, b.BrandCode, cc.ClassName, cc.ClassCode 
                    FROM CarModels cm
                    INNER JOIN Brands b ON cm.BrandID = b.BrandID
                    INNER JOIN CarClasses cc ON cm.ClassID = cc.ClassID
                    WHERE cm.ModelID = @Id";

        var result = await connection.QueryAsync<CarModel, Brand, CarClass, CarModel>(
            sql,
            (model, brand, carClass) =>
            {
                model.Brand = brand;
                model.CarClass = carClass;
                return model;
            },
            new { Id = id },
            splitOn: "BrandID,ClassID"
        );

        return result.FirstOrDefault();
    }

    public async Task<CarModel?> GetByModelCodeAsync(string modelCode)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "SELECT * FROM CarModels WHERE ModelCode = @ModelCode";
        return await connection.QueryFirstOrDefaultAsync<CarModel>(sql, new { ModelCode = modelCode });
    }

    public async Task<int> CreateAsync(CarModel model)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"INSERT INTO CarModels (BrandID, ClassID, ModelName, ModelCode, Description, Features, 
                                          Price, DateOfManufacturing, IsActive, SortOrder, CreatedBy, CreatedOn)
                    VALUES (@BrandID, @ClassID, @ModelName, @ModelCode, @Description, @Features, 
                            @Price, @DateOfManufacturing, @IsActive, @SortOrder, @CreatedBy, @CreatedOn);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

        return await connection.QuerySingleAsync<int>(sql, model);
    }

    public async Task<bool> UpdateAsync(CarModel model)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"UPDATE CarModels 
                    SET BrandID = @BrandID, ClassID = @ClassID, ModelName = @ModelName, 
                        ModelCode = @ModelCode, Description = @Description, Features = @Features,
                        Price = @Price, DateOfManufacturing = @DateOfManufacturing, 
                        IsActive = @IsActive, SortOrder = @SortOrder, 
                        LastUpdatedBy = @LastUpdatedBy, LastUpdatedOn = GETDATE()
                    WHERE ModelID = @ModelID";

        var rowsAffected = await connection.ExecuteAsync(sql, model);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "UPDATE CarModels SET IsActive = 0 WHERE ModelID = @Id";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<CarModelImage>> GetImagesByModelIdAsync(int modelId)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"SELECT * FROM CarModelImages 
                    WHERE ModelID = @ModelId 
                    ORDER BY IsDefault DESC, SortOrder ASC";
        
        return await connection.QueryAsync<CarModelImage>(sql, new { ModelId = modelId });
    }

    public async Task<int> AddImageAsync(CarModelImage image)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"INSERT INTO CarModelImages (ModelID, ImagePath, ImageName, ImageSize, 
                                               IsDefault, SortOrder, CreatedBy, CreatedOn)
                    VALUES (@ModelID, @ImagePath, @ImageName, @ImageSize, 
                            @IsDefault, @SortOrder, @CreatedBy, @CreatedOn);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

        return await connection.QuerySingleAsync<int>(sql, image);
    }

    public async Task<bool> DeleteImageAsync(int imageId)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "DELETE FROM CarModelImages WHERE ImageID = @ImageId";
        var rowsAffected = await connection.ExecuteAsync(sql, new { ImageId = imageId });
        return rowsAffected > 0;
    }

    public async Task<bool> SetDefaultImageAsync(int imageId, int modelId)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        connection.Open();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            await connection.ExecuteAsync(
                "UPDATE CarModelImages SET IsDefault = 0 WHERE ModelID = @ModelId",
                new { ModelId = modelId },
                transaction
            );

            var rowsAffected = await connection.ExecuteAsync(
                "UPDATE CarModelImages SET IsDefault = 1 WHERE ImageID = @ImageId AND ModelID = @ModelId",
                new { ImageId = imageId, ModelId = modelId },
                transaction
            );

            transaction.Commit();
            return rowsAffected > 0;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<Brand?> GetBrandByIdAsync(int brandId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM Brands WHERE BrandID = @BrandId";
        return await connection.QueryFirstOrDefaultAsync<Brand>(sql, new { BrandId = brandId });
    }

    public async Task<CarClass?> GetClassByIdAsync(int classId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM CarClasses WHERE ClassID = @ClassId";
        return await connection.QueryFirstOrDefaultAsync<CarClass>(sql, new { ClassId = classId });
    }

    public async Task<IEnumerable<Brand>> GetAllBrandsAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM Brands WHERE IsActive = 1 ORDER BY BrandName";
        return await connection.QueryAsync<Brand>(sql);
    }

    public async Task<IEnumerable<CarClass>> GetAllClassesAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM CarClasses WHERE IsActive = 1 ORDER BY DisplayOrder";
        return await connection.QueryAsync<CarClass>(sql);
    }
}
