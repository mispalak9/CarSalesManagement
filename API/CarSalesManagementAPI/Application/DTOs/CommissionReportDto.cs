namespace CarSalesManagementAPI.Application.DTOs;

public class CommissionReportDto
{
    public int SalesmanID { get; set; }
    public string SalesmanName { get; set; } = string.Empty;
    public string SalesmanCode { get; set; } = string.Empty;
    public int SaleMonth { get; set; }
    public int SaleYear { get; set; }
    public decimal PreviousYearSales { get; set; }
    public bool BonusEligible { get; set; }
    public List<BrandCommissionDetailDto> BrandDetails { get; set; } = new();
    public decimal TotalFixedCommission { get; set; }
    public decimal TotalPercentageCommission { get; set; }
    public decimal TotalBonusCommission { get; set; }
    public decimal GrandTotalCommission { get; set; }
}

public class BrandCommissionDetailDto
{
    public int BrandID { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public List<ClassCommissionDetailDto> ClassDetails { get; set; } = new();
    public decimal BrandTotalCommission { get; set; }
}

public class ClassCommissionDetailDto
{
    public int ClassID { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int TotalCarsSold { get; set; }
    public decimal TotalSalesAmount { get; set; }
    public decimal FixedCommission { get; set; }
    public decimal PercentageCommission { get; set; }
    public decimal BonusCommission { get; set; }
    public decimal TotalCommission { get; set; }
}
