using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProductManagement.Models;
using ProductManagement.Options;
using ProductManagement.Repositories;
using ProductManagement.Services;

namespace ProductManagement.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class ProductController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly SiteInfo _siteInfo;

    public ProductController(IOptions<SiteInfo> siteInfo, IProductRepository productRepository)
    {
        _productRepository = productRepository;
        _siteInfo = siteInfo.Value;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["WelcomeMessage"] = _siteInfo.WelcomeMessage;
        ViewData["HomeTitle"] = _siteInfo.HomeTitle;

        var products = await _productRepository.GetAllAsync(cancellationToken);
        return View(products);
    }

    [HttpGet("product/{id:int}")]
    public async Task<IActionResult> Detail(int id, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            return NotFound($"Product with ID: {id} not found");
        }
        return View(product);
    }

    [Authorize]
    [HttpGet("product/create")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize]
    [HttpPost("product/create")]
    public async Task<IActionResult> Create(ProductViewModel productViewModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(productViewModel);
        }

        var newProduct = new Product
        {
            Name = productViewModel.Name,
            ImageUrl = productViewModel.ImageUrl,
            Description = productViewModel.Description,
            SkuCode = productViewModel.SkuCode,
            Category = productViewModel.Category,
            UnitPrice = productViewModel.UnitPrice,
        };

        await _productRepository.AddAsync(newProduct, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    [HttpGet("product/update/{id:int}")]
    public async Task<IActionResult> Update(int id, CancellationToken cancellationToken)
    {
        var existingProduct = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (existingProduct == null)
        {
            return NotFound($"Product with ID: {id} not found");
        }

        var updateProductViewModel = new ProductViewModel
        {
            Id = existingProduct.Id,
            Name = existingProduct.Name,
            SkuCode = existingProduct.SkuCode,
            Description = existingProduct.Description,
            UnitPrice = existingProduct.UnitPrice,
            Category = existingProduct.Category,
            ImageUrl = existingProduct.ImageUrl
        };

        return View(updateProductViewModel);
    }

    [Authorize]
    [HttpPost("product/update/{id:int}")]
    public async Task<IActionResult> Update(int id, ProductViewModel productViewModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(productViewModel);
        }

        var product = await _productRepository.GetByIdAsync(productViewModel.Id, cancellationToken);
        if (product == null)
        {
            return NotFound($"Product with ID: {id} not found");
        }

        product.Name = productViewModel.Name;
        product.SkuCode = productViewModel.SkuCode;
        product.Description = productViewModel.Description;
        product.UnitPrice = productViewModel.UnitPrice;
        product.Category = productViewModel.Category;
        product.ImageUrl = productViewModel.ImageUrl;

        await _productRepository.UpdateAsync(product, cancellationToken);

        return RedirectToAction(nameof(Detail), new { id = product.Id });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("product/delete/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            return NotFound($"Product with ID: {id} not found");
        }

        await _productRepository.DeleteAsync(id);

        return RedirectToAction(nameof(Index));

    }

    [HttpGet("product/export")]
    public async Task<IActionResult> Export([FromServices] IExcelExportService excelExportService, CancellationToken cancellationToken)
    {
        // Get all products from the repository
        var products = await _productRepository.GetAllAsync(cancellationToken);

        // use dynamically injected service to export products to Excel
        var fileContent = excelExportService.ExportProductsToExcel(products);

        // Return the Excel file as a downloadable response
        String fileName = $"Products_Export_{DateTime.Now:yyyyMMdd}.xlsx";
        return File(fileContent,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName
        );
    }

    [Route("Error")]
    public IActionResult Error()
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();

        if (exceptionFeature != null)
        {
            // Log the exception here if needed
            var exception = exceptionFeature.Error;

            return StatusCode(500, new
            {
                Message = "An unexpected error occurred.",
                Detail = exception.Message // Remove this in production
            });
        }

        return StatusCode(500, new
        {
            Message = "An unexpected error occurred."
        });
    }
}
