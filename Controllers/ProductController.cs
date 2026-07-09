using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProductManagement.Data;
using ProductManagement.Models;
using ProductManagement.Options;

namespace ProductManagement.Controllers;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly SiteInfo _siteInfo;

    public ProductController(IOptions<SiteInfo> siteInfo, ApplicationDbContext context)
    {
        _context = context;
        _siteInfo = siteInfo.Value;
    }

    public IActionResult Index()
    {
        ViewData["WelcomeMessage"] = _siteInfo.WelcomeMessage;
        ViewData["HomeTitle"] = _siteInfo.HomeTitle;

        var productsFromDb = _context.Products.ToList();
        return View(productsFromDb);
    }

    [HttpGet("product/{id:int}")]
    public IActionResult Detail(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound($"Product with ID: {id} not found");
        }
        return View(product);
    }


    [HttpGet("product/create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("product/create")]
    public IActionResult Create(ProductViewModel productViewModel)
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

        _context.Products.Add(newProduct);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("product/update/{id:int}")]
    public IActionResult Update(int id)
    {
        var existingProduct = _context.Products.FirstOrDefault(p => p.Id == id);
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

    [HttpPost("product/update/{id:int}")]
    public IActionResult Update(int id, ProductViewModel productViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(productViewModel);
        }

        var product = _context.Products.First(p => p.Id == productViewModel.Id);
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

        _context.SaveChanges();

        return RedirectToAction(nameof(Detail), new { id = product.Id });
    }

    [HttpGet("product/delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound($"Product with ID: {id} not found");
        }

        _context.Products.Remove(product);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));

    }

}
