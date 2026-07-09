using ProductManagement.Models;

namespace ProductManagement.Services;

public interface IExcelExportService
{
    byte[] ExportProductsToExcel(IEnumerable<Product> products);
}
