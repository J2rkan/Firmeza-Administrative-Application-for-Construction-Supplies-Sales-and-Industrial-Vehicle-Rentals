namespace Firmeza.Core.Entities;

public class Sale
{
    public int id { get; set; }
    public DateTime Date { get; set; }
    public decimal Total { get; set; }

    public int ClientId { get; set;  }
    public virtual Client Client { get; set; }

    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}