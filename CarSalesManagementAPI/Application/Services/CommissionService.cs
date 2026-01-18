using AutoMapper;
using CarSalesManagementAPI.Application.Constants;
using CarSalesManagementAPI.Application.DTOs;
using CarSalesManagementAPI.Domain.Entities;
using CarSalesManagementAPI.Domain.Interfaces;

namespace CarSalesManagementAPI.Application.Services;

public class CommissionService : ICommissionService
{
    private readonly ICommissionRepository _repository;
    private readonly IMapper _mapper;

    public CommissionService(ICommissionRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CommissionReportDto>> GenerateCommissionReportAsync(int salesmanId, int month, int year)
    {
        try
        {
            var salesman = await _repository.GetSalesmanByIdAsync(salesmanId);
            if (salesman == null)
            {
                return new ApiResponse<CommissionReportDto>
                {
                    Success = false,
                    Message = "Salesman not found.",
                    Errors = new List<string> { $"Salesman with ID {salesmanId} not found." }
                };
            }

            var previousYear = year - 1;
            var yearlySales = await _repository.GetSalesmanYearlySalesAsync(salesmanId, previousYear);
            var previousYearSales = yearlySales?.TotalSaleAmount ?? 0;
            var bonusEligible = previousYearSales > ApplicationConstants.Commission.BonusEligibilityThreshold;

            var sales = await _repository.GetSalesBySalesmanMonthYearAsync(salesmanId, month, year);
            if (!sales.Any())
            {
                return new ApiResponse<CommissionReportDto>
                {
                    Success = true,
                    Message = "No sales found for the specified period.",
                    Data = new CommissionReportDto
                    {
                        SalesmanID = salesman.SalesmanID,
                        SalesmanName = salesman.SalesmanName,
                        SalesmanCode = salesman.SalesmanCode,
                        SaleMonth = month,
                        SaleYear = year,
                        PreviousYearSales = previousYearSales,
                        BonusEligible = bonusEligible
                    }
                };
            }

            // Batch load all required data - only load models that are in the sales
            var modelIds = sales.Select(s => s.ModelID).Distinct().ToList();
            var carModels = await _repository.GetCarModelsByIdsAsync(modelIds);
            var modelsDict = carModels.ToDictionary(m => m.ModelID);
            var allCommissionRules = await _repository.GetAllCommissionRulesAsync();
            var rulesDict = allCommissionRules.ToDictionary(r => (r.BrandID, r.ClassID));

            var report = new CommissionReportDto
            {
                SalesmanID = salesman.SalesmanID,
                SalesmanName = salesman.SalesmanName,
                SalesmanCode = salesman.SalesmanCode,
                SaleMonth = month,
                SaleYear = year,
                PreviousYearSales = previousYearSales,
                BonusEligible = bonusEligible
            };

            // Group sales by brand and class
            var salesByBrandClass = sales
                .Where(s => modelsDict.ContainsKey(s.ModelID))
                .GroupBy(s => new
                {
                    BrandID = modelsDict[s.ModelID].BrandID,
                    BrandName = modelsDict[s.ModelID].Brand?.BrandName ?? "",
                    ClassID = modelsDict[s.ModelID].ClassID,
                    ClassName = modelsDict[s.ModelID].CarClass?.ClassName ?? "",
                    ModelPrice = modelsDict[s.ModelID].Price
                })
                .Select(g => new
                {
                    g.Key.BrandID,
                    g.Key.BrandName,
                    g.Key.ClassID,
                    g.Key.ClassName,
                    g.Key.ModelPrice,
                    TotalCarsSold = g.Sum(s => s.Quantity),
                    TotalSalesAmount = g.Sum(s => s.TotalAmount)
                })
                .ToList();

            var brandGroups = salesByBrandClass.GroupBy(x => new { x.BrandID, x.BrandName });

            foreach (var brandGroup in brandGroups)
            {
                var brandDetail = new BrandCommissionDetailDto
                {
                    BrandID = brandGroup.Key.BrandID,
                    BrandName = brandGroup.Key.BrandName
                };

                foreach (var item in brandGroup)
                {
                    var ruleKey = (item.BrandID, item.ClassID);
                    if (!rulesDict.TryGetValue(ruleKey, out var commissionRule) || commissionRule == null)
                        continue;

                    var classDetail = new ClassCommissionDetailDto
                    {
                        ClassID = item.ClassID,
                        ClassName = item.ClassName,
                        TotalCarsSold = item.TotalCarsSold,
                        TotalSalesAmount = item.TotalSalesAmount
                    };

                    // Calculate Fixed Commission (if price meets minimum requirement)
                    // Fixed commission applies once per brand/class combination if model price meets threshold
                    if (item.ModelPrice >= commissionRule.MinPriceForFixedCommission)
                    {
                        classDetail.FixedCommission = commissionRule.FixedCommission;
                    }

                    // Calculate Percentage Commission (% of total sales amount)
                    classDetail.PercentageCommission = item.TotalSalesAmount * (commissionRule.PercentageCommission / 100);

                    // Calculate Bonus Commission (only for Class A if bonus eligible)
                    // Bonus is 2% of total sales amount for Class A cars only
                    if (bonusEligible && item.ClassID == ApplicationConstants.Commission.ClassAId)
                    {
                        classDetail.BonusCommission = item.TotalSalesAmount * ApplicationConstants.Commission.BonusPercentage;
                    }

                    classDetail.TotalCommission = classDetail.FixedCommission 
                        + classDetail.PercentageCommission 
                        + classDetail.BonusCommission;

                    brandDetail.ClassDetails.Add(classDetail);
                    brandDetail.BrandTotalCommission += classDetail.TotalCommission;

                    // Save commission calculation
                    var commissionCalc = new CommissionCalculation
                    {
                        SalesmanID = salesmanId,
                        SaleMonth = month,
                        SaleYear = year,
                        BrandID = item.BrandID,
                        ClassID = item.ClassID,
                        TotalCarsSold = item.TotalCarsSold,
                        TotalSalesAmount = item.TotalSalesAmount,
                        FixedCommission = classDetail.FixedCommission,
                        PercentageCommission = classDetail.PercentageCommission,
                        BonusCommission = classDetail.BonusCommission,
                        TotalCommission = classDetail.TotalCommission,
                        CalculatedOn = DateTime.Now,
                        CreatedBy = 1,
                        CreatedOn = DateTime.Now
                    };

                    await _repository.SaveCommissionCalculationAsync(commissionCalc);
                }

                report.BrandDetails.Add(brandDetail);
                report.TotalFixedCommission += brandDetail.ClassDetails.Sum(c => c.FixedCommission);
                report.TotalPercentageCommission += brandDetail.ClassDetails.Sum(c => c.PercentageCommission);
                report.TotalBonusCommission += brandDetail.ClassDetails.Sum(c => c.BonusCommission);
                report.GrandTotalCommission += brandDetail.BrandTotalCommission;
            }

            return new ApiResponse<CommissionReportDto>
            {
                Success = true,
                Message = "Commission report generated successfully.",
                Data = report
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<CommissionReportDto>
            {
                Success = false,
                Message = "Error generating commission report.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<IEnumerable<CommissionReportDto>>> GenerateAllSalesmenCommissionReportAsync(int month, int year)
    {
        try
        {
            var salesmen = await _repository.GetAllSalesmenAsync();
            var reports = new List<CommissionReportDto>();

            foreach (var salesman in salesmen)
            {
                var reportResponse = await GenerateCommissionReportAsync(salesman.SalesmanID, month, year);
                if (reportResponse.Success && reportResponse.Data != null)
                {
                    reports.Add(reportResponse.Data);
                }
            }

            return new ApiResponse<IEnumerable<CommissionReportDto>>
            {
                Success = true,
                Message = "Commission reports generated successfully.",
                Data = reports
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<CommissionReportDto>>
            {
                Success = false,
                Message = "Error generating commission reports.",
                Errors = new List<string> { ex.Message }
            };
        }
    }
}
