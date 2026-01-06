namespace ConsoleApp.Fundamentals._06_Inyeccion_Dependencias;

public class ProductManager
{
    private readonly ILabelService _labelService;

    public ProductManager(ILabelService labelService)
    {
        _labelService = labelService;
    }

    public void PrintLabel(Product product)
    {
        var label = _labelService.GenerateLabel(product);
        Console.WriteLine(label);
    }
}