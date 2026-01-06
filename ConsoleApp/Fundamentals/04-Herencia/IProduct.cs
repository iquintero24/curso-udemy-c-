namespace ConsoleApp.Fundamentals;

public interface IProduct
{
    void ApplyDiscount(decimal percentage);
    
    string GetDescription(); 
}