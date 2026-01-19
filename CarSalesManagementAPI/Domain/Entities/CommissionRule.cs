namespace CarSalesManagementAPI.Domain.Entities;

public class CommissionRule
{
    public int RuleID { get; set; }
    public int BrandID { get; set; }
    public int ClassID { get; set; }
    public decimal FixedCommission { get; set; }
    public decimal MinPriceForFixedCommission { get; set; }
    public decimal PercentageCommission { get; set; }
    public bool IsActive { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public int? LastUpdatedBy { get; set; }
    public DateTime? LastUpdatedOn { get; set; }
}
