using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Firmeza.Core.Interfaces;

public interface IExcelParserService
{
    Task<List<Dictionary<string, string>>> ParseExcelDataAsync(Stream fileStream);
}