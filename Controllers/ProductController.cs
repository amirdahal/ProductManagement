using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProductManagement.Data;
using ProductManagement.Models;
using ProductManagement.Options;

namespace ProductManagement.Controllers;

public class ProductController : Controller
{
    private List<Product> _products = new List<Product>();
    private SiteInfo _siteInfo;

    public ProductController(IOptions<SiteInfo> siteInfo)
    {
        _siteInfo = siteInfo.Value;
        _products = SampleProducts.Products;
    }
    // Default Landing Action for the app
    public IActionResult Index()
    {
        var model = _products;
        ViewData["WelcomeMessage"] = _siteInfo.WelcomeMessage;
        ViewData["HomeTitle"] = _siteInfo.HomeTitle;
        return View(model);
    }

    [HttpGet("product/{id:int}")]
    public IActionResult Detail(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
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
        Console.WriteLine($"{productViewModel.Name} | {productViewModel.UnitPrice} | {productViewModel.Category} | {productViewModel.SkuCode} | {productViewModel.Description} | {productViewModel.ImageUrl}");

        if (!ModelState.IsValid) {
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
            Id = _products.Count + 1
        };

        _products.Add(newProduct);

        return RedirectToAction("Index");
    }

    

}
