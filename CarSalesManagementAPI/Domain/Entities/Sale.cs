namespace CarSalesManagementAPI.Domain.Entities;

public class Sale
{
    public int SaleID { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public int SalesmanID { get; set; }
    public int ModelID { get; set; }
    public DateTime SaleDate { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public int SaleMonth { get; set; }
    public int SaleYear { get; set; }
    public string Status { get; set; } = "Completed";
    public int? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public int? LastUpdatedBy { get; set; }
    public DateTime? LastUpdatedOn { get; set; }
}
