namespace Firmeza.Core.Interfaces;

public interface IExcelParserService
{
    Task<List<Dictionary<string, string>>> ParseExcelDataAsync(Stream fileStream);
}