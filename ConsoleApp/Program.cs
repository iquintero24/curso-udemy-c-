using ConsoleApp.Fundamentals;
using ConsoleApp.Fundamentals._05_Patron_Adaptador;
using ConsoleApp.Fundamentals._06_Inyeccion_Dependencias;
using ConsoleApp.Fundamentals._07_Metodos_Asincronos;
using ConsoleApp.Fundamentals._08_Atributos;

// Console.WriteLine("Hello, World!");

// type of data

var laptop = new Product("laptop", 1200);
// Console.WriteLine(laptop.GetDescription()); 

var support = new ServiceProduct("soporte Tecnico", 1200, 30);
// Console.WriteLine(support.GetDescription()); 

var product = new Product("Mouse", 1200);
var productDto = ProductAdapter.ToDto(product);

// Console.WriteLine($"{productDto.Name} - {productDto.Price:C} - Codigo : {productDto.Code}");

// inyeccion de dependecias

ILabelService labelService = new LabelService();
var manager = new ProductManager(labelService);

// creacion de productos 
var monitor = new Product("Monitor", 1500);
var installation = new ServiceProduct("Instalacion de monitor", 1000, 30);

//manager.PrintLabel(monitor);
// manager.PrintLabel(installation);

// async call
var fistProduct = await new ProductRepository().GetProduct(1);

AttributeProcessor.ApplyUpperCase(fistProduct);
Console.WriteLine($"{fistProduct.Name} - {fistProduct.Price}");