namespace CarSalesManagementAPI.Domain.Entities;

public class CommissionCalculation
{
    public int CommissionID { get; set; }
    public int SalesmanID { get; set; }
    public int SaleMonth { get; set; }
    public int SaleYear { get; set; }
    public int BrandID { get; set; }
    public int ClassID { get; set; }
    public int TotalCarsSold { get; set; }
    public decimal TotalSalesAmount { get; set; }
    public decimal FixedCommission { get; set; }
    public decimal PercentageCommission { get; set; }
    public decimal BonusCommission { get; set; }
    public decimal TotalCommission { get; set; }
    public DateTime CalculatedOn { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
}
