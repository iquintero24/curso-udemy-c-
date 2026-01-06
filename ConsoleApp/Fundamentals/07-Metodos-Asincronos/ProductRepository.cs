namespace ConsoleApp.Fundamentals._07_Metodos_Asincronos;

public class ProductRepository
{
    public async Task<Product> GetProduct(int id)
    {
        // obtener de una base de datos, llmada a un api externa
        
        Console.WriteLine($"Getting product with id {id}");
        await Task.Delay(2000);
        return new Product("Producto simulado", 500);
    }
}