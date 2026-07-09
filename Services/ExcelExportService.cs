using OfficeOpenXml;
using ProductManagement.Models;

namespace ProductManagement.Services;

public class ExcelExportService : IExcelExportService
{
    public byte[] ExportProductsToExcel(IEnumerable<Product> products)
    {
        // Set the license for EPPlus 8+ (replaces obsolete LicenseContext)
        ExcelPackage.License.SetNonCommercialPersonal("Amir Dahal");

        using (var package = new ExcelPackage())
        {
            // Add new worksheet to the empty workbook
            var worksheet = package.Workbook.Worksheets.Add("Products");

            // 1. Add the headers
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Name";
            worksheet.Cells[1, 3].Value = "SKU Code";
            worksheet.Cells[1, 4].Value = "Category";
            worksheet.Cells[1, 5].Value = "Unit Price";
            worksheet.Cells[1, 6].Value = "Image URL";
            worksheet.Cells[1, 7].Value = "Description";

            // Style headers
            using (var range = worksheet.Cells[1, 1, 1, 7])
            {
                range.Style.Font.Bold = true;
            }

            //2.Populate Data rows
            int row = 2; // Start from the second row
            foreach (var product in products)
            {
                worksheet.Cells[row, 1].Value = product.Id;
                worksheet.Cells[row, 2].Value = product.Name;
                worksheet.Cells[row, 3].Value = product.SkuCode;
                worksheet.Cells[row, 4].Value = product.Category;
                worksheet.Cells[row, 5].Value = product.UnitPrice;
                worksheet.Cells[row, 6].Value = product.ImageUrl;
                worksheet.Cells[row, 7].Value = product.Description;
                row++;
            }

            // Auto-fit columns for neatness
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // Convert the Excel package to a byte array and return it
            return package.GetAsByteArray();
        }
    }
}
