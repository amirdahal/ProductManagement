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

        return RedirectToAction("Index");
    }

}
