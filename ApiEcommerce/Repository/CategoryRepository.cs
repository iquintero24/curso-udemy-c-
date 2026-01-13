using ApiEcommerce.Data;
using ApiEcommerce.Models;
using ApiEcommerce.Repository.IRepository;

namespace ApiEcommerce.Repository;

public class CategoryRepository: ICategoryRepository
{
    
    private readonly ApplicationDbContext _dbContext;

    public CategoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public ICollection<Category> GetCategories()
    {
        return _dbContext.Categories.OrderBy(c => c.Name).ToList();
    }

    public Category? GetCategoryById(int id)
    {
        return _dbContext.Categories.FirstOrDefault(c => c.Id == id);

    }

    public bool CategoryExists(int id)
    {
        return _dbContext.Categories.Any(c => c.Id == id);
    }

    public bool CategoryExists(string name)
    {
        return _dbContext.Categories.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public bool CreateCategory(Category category)
    {
        category.CreatedAt = DateTime.Now;
        _dbContext.Categories.Add(category);
        return Save();
    }

    public bool UpdateCategory(Category category)
    {
        category.CreatedAt = DateTime.Now;
        _dbContext.Categories.Update(category);
        return Save();
    }

    public bool DeleteCategory(Category category)
    {
        _dbContext.Categories.Remove(category);
        return Save();
    }

    public bool Save()
    {
       return _dbContext.SaveChanges() >=  0 ? true : false;
    }
}