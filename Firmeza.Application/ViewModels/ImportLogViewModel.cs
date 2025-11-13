namespace Firmeza.Application.ViewModels
{
    public class ImportLogViewModel
    {
        public bool Success { get; set; }
        public int RecordsProcessed { get; set; }
        public int SalesImported { get; set; }
        public int NewClientsCreated { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
    }
}