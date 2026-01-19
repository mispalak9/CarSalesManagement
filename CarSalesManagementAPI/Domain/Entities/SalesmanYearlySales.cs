namespace CarSalesManagementAPI.Domain.Entities;

public class SalesmanYearlySales
{
    public int YearlySaleID { get; set; }
    public int SalesmanID { get; set; }
    public int SaleYear { get; set; }
    public decimal TotalSaleAmount { get; set; }
    public bool BonusEligible { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public int? LastUpdatedBy { get; set; }
    public DateTime? LastUpdatedOn { get; set; }
}
