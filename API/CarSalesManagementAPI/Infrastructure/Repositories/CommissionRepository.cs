using Dapper;
using CarSalesManagementAPI.Domain.Entities;
using CarSalesManagementAPI.Domain.Interfaces;
using CarSalesManagementAPI.Infrastructure.Data;
using System.Data;

namespace CarSalesManagementAPI.Infrastructure.Repositories;

public class CommissionRepository : ICommissionRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CommissionRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Salesman>> GetAllSalesmen()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM Salesmen WHERE IsActive = 1 ORDER BY SalesmanName";
        return await connection.QueryAsync<Salesman>(sql);
    }

    public async Task<Salesman?> GetSalesmanById(int salesmanId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM Salesmen WHERE SalesmanID = @SalesmanId";
        return await connection.QueryFirstOrDefaultAsync<Salesman>(sql, new { SalesmanId = salesmanId });
    }

    public async Task<IEnumerable<Sale>> GetSalesBySalesmanMonthYear(int salesmanId, int month, int year)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"SELECT * FROM Sales 
                    WHERE SalesmanID = @SalesmanId 
                    AND SaleMonth = @Month 
                    AND SaleYear = @Year 
                    AND Status = 'Completed'
                    ORDER BY SaleDate";
        return await connection.QueryAsync<Sale>(sql, new { SalesmanId = salesmanId, Month = month, Year = year });
    }

    public async Task<CommissionRule?> GetCommissionRule(int brandId, int classId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"SELECT * FROM CommissionRules 
                    WHERE BrandID = @BrandId 
                    AND ClassID = @ClassId 
                    AND IsActive = 1";
        return await connection.QueryFirstOrDefaultAsync<CommissionRule>(sql, new { BrandId = brandId, ClassId = classId });
    }

    public async Task<IEnumerable<CommissionRule>> GetAllCommissionRules()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM CommissionRules WHERE IsActive = 1";
        return await connection.QueryAsync<CommissionRule>(sql);
    }

    public async Task<SalesmanYearlySales?> GetSalesmanYearlySales(int salesmanId, int year)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"SELECT * FROM SalesmanYearlySales 
                    WHERE SalesmanID = @SalesmanId 
                    AND SaleYear = @Year";
        return await connection.QueryFirstOrDefaultAsync<SalesmanYearlySales>(sql, new { SalesmanId = salesmanId, Year = year });
    }

    public async Task<IEnumerable<CommissionCalculation>> GetCommissionCalculations(int salesmanId, int month, int year)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"SELECT * FROM CommissionCalculations 
                    WHERE SalesmanID = @SalesmanId 
                    AND SaleMonth = @Month 
                    AND SaleYear = @Year
                    ORDER BY BrandID, ClassID";
        return await connection.QueryAsync<CommissionCalculation>(sql, new { SalesmanId = salesmanId, Month = month, Year = year });
    }

    public async Task<int> SaveCommissionCalculation(CommissionCalculation calculation)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"IF EXISTS (SELECT 1 FROM CommissionCalculations 
                    WHERE SalesmanID = @SalesmanID AND SaleMonth = @SaleMonth 
                    AND SaleYear = @SaleYear AND BrandID = @BrandID AND ClassID = @ClassID)
                    BEGIN
                        UPDATE CommissionCalculations 
                        SET TotalCarsSold = @TotalCarsSold,
                            TotalSalesAmount = @TotalSalesAmount,
                            FixedCommission = @FixedCommission,
                            PercentageCommission = @PercentageCommission,
                            BonusCommission = @BonusCommission,
                            TotalCommission = @TotalCommission,
                            CalculatedOn = GETDATE()
                        WHERE SalesmanID = @SalesmanID AND SaleMonth = @SaleMonth 
                        AND SaleYear = @SaleYear AND BrandID = @BrandID AND ClassID = @ClassID;
                        SELECT CommissionID FROM CommissionCalculations 
                        WHERE SalesmanID = @SalesmanID AND SaleMonth = @SaleMonth 
                        AND SaleYear = @SaleYear AND BrandID = @BrandID AND ClassID = @ClassID;
                    END
                    ELSE
                    BEGIN
                        INSERT INTO CommissionCalculations 
                        (SalesmanID, SaleMonth, SaleYear, BrandID, ClassID, TotalCarsSold, 
                         TotalSalesAmount, FixedCommission, PercentageCommission, BonusCommission, 
                         TotalCommission, CalculatedOn, CreatedBy, CreatedOn)
                        VALUES 
                        (@SalesmanID, @SaleMonth, @SaleYear, @BrandID, @ClassID, @TotalCarsSold,
                         @TotalSalesAmount, @FixedCommission, @PercentageCommission, @BonusCommission,
                         @TotalCommission, GETDATE(), @CreatedBy, GETDATE());
                        SELECT CAST(SCOPE_IDENTITY() as int);
                    END";

        return await connection.QuerySingleAsync<int>(sql, calculation);
    }

    public async Task<IEnumerable<CarModel>> GetCarModels()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"SELECT cm.*, b.BrandID, b.BrandName, b.BrandCode, 
                           cc.ClassID, cc.ClassName, cc.ClassCode 
                    FROM CarModels cm
                    INNER JOIN Brands b ON cm.BrandID = b.BrandID
                    INNER JOIN CarClasses cc ON cm.ClassID = cc.ClassID
                    WHERE cm.IsActive = 1";
        
        return await connection.QueryAsync<CarModel, Brand, CarClass, CarModel>(
            sql,
            (model, brand, carClass) =>
            {
                model.Brand = brand;
                model.CarClass = carClass;
                return model;
            },
            splitOn: "BrandName,ClassName"
        );
    }

    public async Task<IEnumerable<CarModel>> GetCarModelsByIds(IEnumerable<int> modelIds)
    {
        var idsList = modelIds.ToList();
        if (!idsList.Any())
            return Enumerable.Empty<CarModel>();

        using var connection = _connectionFactory.CreateConnection();
        var sql = @"SELECT cm.*, b.BrandID, b.BrandName, b.BrandCode, 
                           cc.ClassID, cc.ClassName, cc.ClassCode 
                    FROM CarModels cm
                    INNER JOIN Brands b ON cm.BrandID = b.BrandID
                    INNER JOIN CarClasses cc ON cm.ClassID = cc.ClassID
                    WHERE cm.IsActive = 1 AND cm.ModelID IN @ModelIds";
        
        return await connection.QueryAsync<CarModel, Brand, CarClass, CarModel>(
            sql,
            (model, brand, carClass) =>
            {
                model.Brand = brand;
                model.CarClass = carClass;
                return model;
            },
            new { ModelIds = idsList },
            splitOn: "BrandName,ClassName"
        );
    }

    public async Task<IEnumerable<CarClass>> GetAllCarClasses()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM CarClasses WHERE IsActive = 1 ORDER BY DisplayOrder";
        return await connection.QueryAsync<CarClass>(sql);
    }
}
