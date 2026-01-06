namespace ConsoleApp.Fundamentals._08_Atributos;

[AttributeUsage(AttributeTargets.Property)]
public class UpperCaseAttribute : Attribute
{
}

public static class AttributeProcessor
{
    public static void ApplyUpperCase(object obj)
    {
        var props = obj.GetType().GetProperties();

        foreach (var prop in props)
        {
            // Verifica que la propiedad tenga el atributo y sea tipo string
            if (prop.PropertyType == typeof(string) &&
                Attribute.IsDefined(prop, typeof(UpperCaseAttribute)))
            {
                var value = prop.GetValue(obj) as string;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    // Establece el valor en mayúsculas
                    prop.SetValue(obj, value.ToUpper());
                }
            }
        }
    }
}