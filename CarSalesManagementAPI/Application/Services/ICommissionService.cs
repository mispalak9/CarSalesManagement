using CarSalesManagementAPI.Application.DTOs;

namespace CarSalesManagementAPI.Application.Services;

public interface ICommissionService
{
    Task<ApiResponse<CommissionReportDto>> GenerateCommissionReportAsync(int salesmanId, int month, int year);
    Task<ApiResponse<IEnumerable<CommissionReportDto>>> GenerateAllSalesmenCommissionReportAsync(int month, int year);
}
