namespace Firmeza.Core.Entities;

public class SaleDetail
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    
    public int ProductId { get; set; }
    public virtual Product Product { get; set; }


}