using CarSalesManagementAPI.Domain.Entities;

namespace CarSalesManagementAPI.Domain.Interfaces;

public interface ICommissionRepository
{
    Task<IEnumerable<Salesman>> GetAllSalesmen();
    Task<Salesman?> GetSalesmanById(int salesmanId);
    Task<IEnumerable<Sale>> GetSalesBySalesmanMonthYear(int salesmanId, int month, int year);
    Task<CommissionRule?> GetCommissionRule(int brandId, int classId);
    Task<IEnumerable<CommissionRule>> GetAllCommissionRules();
    Task<SalesmanYearlySales?> GetSalesmanYearlySales(int salesmanId, int year);
    Task<IEnumerable<CommissionCalculation>> GetCommissionCalculations(int salesmanId, int month, int year);
    Task<int> SaveCommissionCalculation(CommissionCalculation calculation);
    Task<IEnumerable<CarModel>> GetCarModels();
    Task<IEnumerable<CarModel>> GetCarModelsByIds(IEnumerable<int> modelIds);
    Task<IEnumerable<CarClass>> GetAllCarClasses();
}
