using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Models;
using ProductManagement.Repositories;

namespace ProductManagement.Controllers.Api;

[ApiVersion("1.0")]
[ApiController]
// This route becomes /api/v1/products
[Route("api/v{version:apiVersion}/products")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProductApiController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductApiController(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    // GET All: api/products
    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetProducts()
    {
        var products = _productRepository.GetAll();
        return Ok(products); // Returns raw json instead of HTML view
    }

    /// Get by Id: api/products/{id}
    [HttpGet("{id}", Name = "GetProductById")]
    public ActionResult<Product> GetById(int id)
    {
        var product = _productRepository.GetById(id);
        if (product == null)
        {
            return NotFound(new { message = $"Product with ID {id} not found." });
        }
        return Ok(product);
    }

    // Create product: api/products
    [HttpPost]
    public ActionResult<Product> Create([FromBody] ProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newProduct = _mapper.Map<Product>(model);

        //  REPLACED WITH AUTOMAPPER
        //var newProduct = new Product
        //{
        //    Name = model.Name,
        //    Category = model.Category,
        //    Description = model.Description,
        //    SkuCode = model.SkuCode,
        //    UnitPrice = model.UnitPrice,
        //    ImageUrl = model.ImageUrl,
        //};
        _productRepository.Add(newProduct);

        return CreatedAtRoute("GetProductById", new { id = newProduct.Id }, newProduct);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] ProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingProduct = _productRepository.GetById(id);
        if (existingProduct == null)
        {
            return NotFound(new { message = $"Product with ID {id} cannot be updated." });
        }

        model.Id = id;

        _mapper.Map(model, existingProduct);

        _productRepository.Update(existingProduct);

        return NoContent(); // returns 204 not content (standard response for PUT/Update)
    }

    // DELETE: api/products/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var existingProduct = _productRepository.GetById(id);
        if (existingProduct == null)
        {
            return NotFound(new { message = $"Product with ID {id} cannot be deleted because it does not exist." });
        }

        _productRepository.Delete(existingProduct.Id);

        return Ok(new { message = $"Product {id} successfully deleted" });
    }
}

/*
    ActionResult<T> : Suitable when an action returns a known data ActionResult<Product>
    IActionResult   : Suitable for actions that does not return a data/may return varying responses 204,401,403
 
*/