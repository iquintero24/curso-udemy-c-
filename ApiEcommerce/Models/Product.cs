using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiEcommerce.Models;

public class Product
{
    [Key] 
    public int ProductId { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
    public string ImageUrl { get; set; } = string.Empty;
    
    [Required]
    public string SKU { get; set; } = string.Empty; // PROD-001-BLK-M
    
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; } = null;
    
    // relacion con el modelo categoria
    
    public int CategoryId { get; set; }
    [ForeignKey("Id")]
    
    public required Category Category { get; set; }
    
}