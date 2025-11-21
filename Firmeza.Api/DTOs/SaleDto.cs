namespace Firmeza.Api.DTOs;

public class SaleDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Total { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public List<SaleDetailDto> SaleDetails { get; set; } = new();
}

public class SaleDetailDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}

public class CreateSaleDto
{
    public int ClientId { get; set; }
    public List<CreateSaleDetailDto> SaleDetails { get; set; } = new();
}

public class CreateSaleDetailDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
