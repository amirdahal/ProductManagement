using AutoMapper;
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
    public readonly IMapper _mapper;
    private readonly SiteInfo _siteInfo;

    public ProductController(IOptions<SiteInfo> siteInfo, IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
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

        var newProduct = _mapper.Map<Product>(productViewModel);

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

        var updateProductViewModel = _mapper.Map<ProductViewModel>(existingProduct);

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

        var existingProduct = await _productRepository.GetByIdAsync(productViewModel.Id, cancellationToken);
        if (existingProduct == null)
        {
            return NotFound($"Product with ID: {id} not found");
        }

        productViewModel.Id = id;
        _mapper.Map(productViewModel, existingProduct);

        await _productRepository.UpdateAsync(existingProduct, cancellationToken);

        return RedirectToAction(nameof(Detail), new { id = existingProduct.Id });
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
