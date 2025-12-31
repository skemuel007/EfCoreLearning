namespace EfCoreLearning.models;

public sealed class OrderWithItemPriceRow
{
    public int OrderID { get; set; }
    public int CustomerID { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal ShipAmount { get; set; }
    public decimal TaxAmount { get; set; }
    
    public decimal ItemPrice { get; set; }
}