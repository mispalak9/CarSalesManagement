using CarSalesManagementAPI.Application.DTOs;

namespace CarSalesManagementAPI.Application.Services;

public interface ICommissionService
{
    Task<ApiResponse<CommissionReportDto>> GenerateCommissionReport(int salesmanId, int month, int year);
    Task<ApiResponse<IEnumerable<CommissionReportDto>>> GenerateAllSalesmenCommissionReport(int month, int year);
}
