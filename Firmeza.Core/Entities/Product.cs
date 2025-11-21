namespace Firmeza.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();

        public void AddStock(int quantity)
        {
            if (quantity < 0) throw new ArgumentException("Quantity cannot be negative");
            Stock += quantity;
        }

        public void RemoveStock(int quantity)
        {
            if (quantity < 0) throw new ArgumentException("Quantity cannot be negative");
            if (Stock < quantity) throw new InvalidOperationException("Insufficient stock");
            Stock -= quantity;
        }
    }
}