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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return Ok(products);
    }

    /// Get by Id: api/products/{id}
    [HttpGet("{id}", Name = "GetProductById")]
    public async Task<ActionResult<Product>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            return NotFound(new { message = $"Product with ID {id} not found." });
        }
        return Ok(product);
    }

    // Create product: api/products
    [HttpPost]
    public async Task<ActionResult<Product>> Create([FromBody] ProductViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newProduct = _mapper.Map<Product>(model);

        await _productRepository.AddAsync(newProduct, cancellationToken);

        return CreatedAtRoute("GetProductById", new { id = newProduct.Id }, newProduct);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingProduct = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (existingProduct == null)
        {
            return NotFound(new { message = $"Product with ID {id} cannot be updated." });
        }

        model.Id = id;

        _mapper.Map(model, existingProduct);

        await _productRepository.UpdateAsync(existingProduct, cancellationToken);

        return NoContent(); // returns 204 not content (standard response for PUT/Update)
    }

    // DELETE: api/products/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var existingProduct = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (existingProduct == null)
        {
            return NotFound(new { message = $"Product with ID {id} cannot be deleted because it does not exist." });
        }

        await _productRepository.DeleteAsync(existingProduct.Id, cancellationToken);

        return Ok(new { message = $"Product {id} successfully deleted" });
    }
}

/*
    ActionResult<T> : Suitable when an action returns a known data ActionResult<Product>
    IActionResult   : Suitable for actions that does not return a data/may return varying responses 204,401,403
 
*/