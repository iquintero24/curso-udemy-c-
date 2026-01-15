using ApiEcommerce.Data;
using ApiEcommerce.Models;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiEcommerce.Repository;

public class ProductRepository:IProductRepository
{
    private readonly ApplicationDbContext _dbContext;
    

    public ProductRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public ICollection<Product> GetAll()
    {
        return _dbContext.Products.Include(p=>p.Category).OrderBy(c => c.Name).ToList();
    }

    public ICollection<Product> GetProductsByCategoryId(int categoryId)
    {
        if (categoryId <= 0)
        {
            return new List<Product>();
        }
        
        return _dbContext.Products.Include(p=> p.Category).Where(p=> p.CategoryId == categoryId).OrderBy(p => p.Name).ToList();
    }

    public ICollection<Product> SearchṔroducts(string searchTerm)
    {
        
        var searchTermLower = searchTerm.ToLower().Trim();
        
        IQueryable<Product> query = _dbContext.Products;
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Include(p => p.Category).Where(p =>
                p.Name.ToLower().Trim().Contains(searchTermLower) ||
                p.Description.ToLower().Trim().Contains(searchTermLower));
        }
        
        return query.OrderBy(p => p.Name).ToList();
    }
    

    public Product? GetById(int id)
    {
        if (id <= 0)
        {
            return null;
        }
        
        return _dbContext.Products.Include(p=>p.Category).FirstOrDefault(p => p.ProductId == id); 
    }

    public bool BuyProduct(string productName, int quantity)
    {
        if (string.IsNullOrWhiteSpace(productName) || quantity <= 0)
        {
            return false;
        }
        
        var product = _dbContext.Products.FirstOrDefault(p => p.Name.ToLower().Trim() == productName.ToLower().Trim());
        if (product == null || product.Stock < quantity)
        {
            return false;
        }
        product.Stock -= quantity;  
        _dbContext.Update(product);
        _dbContext.SaveChanges();
        return true; 
    }

    public bool ProductExists(int id)
    {
        if (id <= 0)
        {
            return false;
        }
        
        return _dbContext.Products.Any(p => p.ProductId == id);
    }

    public bool ProductExits(string productName)
    {
        if (string.IsNullOrWhiteSpace(productName))
        {
            return false;
        }
        return _dbContext.Products.Any(p => p.Name.ToLower().Trim() == productName.ToLower().Trim() );
    }

    public bool CreateProduct(Product product)
    {
        if (product == null)
        {
            return false;
        }
        
        product.CreatedAt = DateTime.Now;
        _dbContext.Products.Add(product);
        return Save();

}

    public bool UpdateProduct(Product product)
    {
        if (product == null)
        {
            return false;
        }

        product.UpdatedAt = DateTime.Now;

        _dbContext.Products.Update(product);

        return Save();
    }

    public bool DeleteProduct(Product product)
    {
        if (product == null)
        {
            return false;
        }
    
        _dbContext.Products.Remove(product);
        return Save();
    }

    public bool Save()
    {
        return  _dbContext.SaveChanges() >= 0;
    }
}