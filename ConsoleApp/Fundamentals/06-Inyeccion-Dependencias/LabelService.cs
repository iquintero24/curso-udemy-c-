namespace ConsoleApp.Fundamentals._06_Inyeccion_Dependencias;

public class LabelService: ILabelService
{
    public string GenerateLabel(Product product)
    {
        return $"{product.Name} - Precio: {product.Price} - Codigo: {product.GetType().Name}-{product.GetHashCode()}";
    }
}