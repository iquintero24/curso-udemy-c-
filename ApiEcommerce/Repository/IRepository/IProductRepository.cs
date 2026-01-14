using ApiEcommerce.Models;

namespace ApiEcommerce.Repository.IRepository;

public interface IProductRepository
{
    ICollection<Product> GetAll();
    ICollection<Product> GetProductsByCategoryId(int categoryId);
    ICollection<Product> SearchṔroduct(string name);
    Product? GetById(int id);
    bool BuyProduct(string productName, int quantity);
    bool ProductExists(int id);
    bool ProductExits(string productName);
    bool CreateProduct(Product product);
    bool UpdateProduct(Product product);
    bool DeleteProduct(Product product);
    bool Save();
}