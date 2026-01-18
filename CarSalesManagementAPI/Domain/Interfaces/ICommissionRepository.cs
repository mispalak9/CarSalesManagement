using CarSalesManagementAPI.Domain.Entities;

namespace CarSalesManagementAPI.Domain.Interfaces;

public interface ICommissionRepository
{
    Task<IEnumerable<Salesman>> GetAllSalesmenAsync();
    Task<Salesman?> GetSalesmanByIdAsync(int salesmanId);
    Task<IEnumerable<Sale>> GetSalesBySalesmanMonthYearAsync(int salesmanId, int month, int year);
    Task<CommissionRule?> GetCommissionRuleAsync(int brandId, int classId);
    Task<IEnumerable<CommissionRule>> GetAllCommissionRulesAsync();
    Task<SalesmanYearlySales?> GetSalesmanYearlySalesAsync(int salesmanId, int year);
    Task<IEnumerable<CommissionCalculation>> GetCommissionCalculationsAsync(int salesmanId, int month, int year);
    Task<int> SaveCommissionCalculationAsync(CommissionCalculation calculation);
    Task<IEnumerable<CarModel>> GetCarModelsAsync();
    Task<IEnumerable<CarModel>> GetCarModelsByIdsAsync(IEnumerable<int> modelIds);
}
