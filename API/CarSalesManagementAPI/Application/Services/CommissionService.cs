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
    private readonly ICacheService _cacheService;

    public CommissionService(ICommissionRepository repository, IMapper mapper, ICacheService cacheService)
    {
        _repository = repository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<CommissionReportDto>> GenerateCommissionReport(int salesmanId, int month, int year)
    {
        try
        {
            var salesman = await _repository.GetSalesmanById(salesmanId);
            if (salesman == null)
            {
                return new ApiResponse<CommissionReportDto>
                {
                    Success = false,
                    Message = "Salesman not found.",
                    Errors = new List<string> { "Salesman ID not found." }
                };
            }

            var sales = await _repository.GetSalesBySalesmanMonthYear(salesmanId, month, year);
            if (!sales.Any())
            {
                return new ApiResponse<CommissionReportDto>
                {
                    Success = true,
                    Message = "No sales found for the specified period.",
                    Data = new CommissionReportDto
                    {
                        SalesmanID = salesmanId,
                        SalesmanName = salesman.SalesmanName,
                        SalesmanCode = salesman.SalesmanCode,
                        SaleMonth = month,
                        SaleYear = year,
                        PreviousYearSales = 0,
                        BonusEligible = false,
                        BrandDetails = new List<BrandCommissionDetailDto>(),
                        TotalFixedCommission = 0,
                        TotalPercentageCommission = 0,
                        TotalBonusCommission = 0,
                        GrandTotalCommission = 0
                    }
                };
            }

            var previousYearSales = await _repository.GetSalesmanYearlySales(salesmanId, year - 1);
            var bonusEligible = previousYearSales?.TotalSaleAmount >= ApplicationConstants.Commission.BonusEligibilityThreshold;

            var modelIds = sales.Select(s => s.ModelID).Distinct().ToList();
            var carModels = await _repository.GetCarModelsByIds(modelIds);
            var modelsDict = carModels.ToDictionary(m => m.ModelID);

            var allCommissionRules = await _cacheService.GetCommissionRules();
            var rulesDict = allCommissionRules.ToDictionary(r => $"{r.BrandID}_{r.ClassID}");

            var carClasses = await _cacheService.GetCarClasses();
            var classAId = carClasses.FirstOrDefault(c => c.ClassCode == "A")?.ClassID ?? 0;

            var report = new CommissionReportDto
            {
                SalesmanID = salesmanId,
                SalesmanName = salesman.SalesmanName,
                SalesmanCode = salesman.SalesmanCode,
                SaleMonth = month,
                SaleYear = year,
                PreviousYearSales = previousYearSales?.TotalSaleAmount ?? 0,
                BonusEligible = bonusEligible
            };

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
                    var ruleKey = $"{item.BrandID}_{item.ClassID}";
                    if (!rulesDict.TryGetValue(ruleKey, out var commissionRule) || commissionRule == null)
                        continue;

                    var classDetail = new ClassCommissionDetailDto
                    {
                        ClassID = item.ClassID,
                        ClassName = item.ClassName,
                        TotalCarsSold = item.TotalCarsSold,
                        TotalSalesAmount = item.TotalSalesAmount
                    };

                    if (item.ModelPrice >= commissionRule.MinPriceForFixedCommission)
                    {
                        classDetail.FixedCommission = commissionRule.FixedCommission;
                    }

                    classDetail.PercentageCommission = item.TotalSalesAmount * (commissionRule.PercentageCommission / 100);

                    if (bonusEligible && item.ClassID == classAId)
                    {
                        classDetail.BonusCommission = item.TotalSalesAmount * ApplicationConstants.Commission.BonusPercentage;
                    }

                    classDetail.TotalCommission = classDetail.FixedCommission 
                        + classDetail.PercentageCommission 
                        + classDetail.BonusCommission;

                    brandDetail.ClassDetails.Add(classDetail);
                    brandDetail.BrandTotalCommission += classDetail.TotalCommission;

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
                        CreatedBy = null,
                        CreatedOn = DateTime.Now
                    };

                    await _repository.SaveCommissionCalculation(commissionCalc);
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

    public async Task<ApiResponse<IEnumerable<CommissionReportDto>>> GenerateAllSalesmenCommissionReport(int month, int year)
    {
        try
        {
            var salesmen = await _repository.GetAllSalesmen();
            var reports = new List<CommissionReportDto>();

            foreach (var salesman in salesmen)
            {
                var reportResponse = await GenerateCommissionReport(salesman.SalesmanID, month, year);
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
