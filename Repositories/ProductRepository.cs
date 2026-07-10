using ProductManagement.Data;
using ProductManagement.Models;
namespace ProductManagement.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Product> GetAll()
    {
        return _context.Products.ToList();
    }

    public Product GetById(int id)
    {
        return _context.Products.Find(id)!;
    }

    public void Add(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();
        //product.Id = trackedEntry.Entity.Id;
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var product = GetById(id);
        if (product == null)
        {
            throw new ArgumentException($"Product with ID: {id} not found");
        }
        _context.Products.Remove(product);
        _context.SaveChanges();
    }
}
