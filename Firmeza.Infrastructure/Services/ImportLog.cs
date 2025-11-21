namespace Firmeza.Infrastructure.Services
{
    public class ImportLog
    {
        public int ProductsImported { get; set; }
        public int ClientsImported { get; set; }
        public int SalesImported { get; set; }
        public int SaleDetailsImported { get; set; }
        public DateTime ImportDate { get; set; }
        public string TotalTime { get; set; }
        public List<ErrorDetail> Errors { get; set; } = new List<ErrorDetail>();
        public bool HasErrors => Errors.Count > 0;
    }

    public class ErrorDetail
    {
        public string EntityType { get; set; }
        public string Message { get; set; }
    }
}