using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers;

[Route("api/[controller]")] // http:localhost:1445/products
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository; // injection de category repository
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper; // injection del mapper 

    // constructor principal
    public ProductsController(IProductRepository productRepository, IMapper mapper, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProduct()
    {
        var products = _productRepository.GetAll();
        var productsDto = _mapper.Map<List<ProductDto>>(products);
        return Ok(productsDto);
    }

    [HttpGet("{productId:int}", Name = "GetProduct")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProduct(int productId)
    {
        var product = _productRepository.GetById(productId);
        if (product == null)
        {
            return NotFound($"El producto con el productId {productId} no existe");
        }
        
        var productDto = _mapper.Map<ProductDto>(product);
        return Ok(productDto);
    }
 
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        if (createProductDto == null)
        {
            return BadRequest(ModelState);
        }

        if (_productRepository.ProductExits(createProductDto.Name))
        {
            ModelState.AddModelError("CustomError", "el producto existe");
            return BadRequest(ModelState);
        }
        
        if (!_categoryRepository.CategoryExists(createProductDto.CategoryId))
        {
            ModelState.AddModelError("CustomError", $"la categoria con el id {createProductDto.CategoryId} no existe");
            return BadRequest(ModelState);
        }
        
        var product = _mapper.Map<Product>(createProductDto);
        if (!_productRepository.CreateProduct(product))
        {
            ModelState.AddModelError("CustomError", $"Algo salio mal al guardar el registro {product.Name}");
            return StatusCode(500, ModelState);
        }
        var createdProduct = _productRepository.GetById(product.ProductId);
        var productDto = _mapper.Map<ProductDto>(createdProduct);
        return CreatedAtRoute("GetProduct", new { productId = product.ProductId  }, productDto); 
        
    }
    
    [HttpGet("searchProductByCategory/{categoryId:int}", Name = "GetProductsByCategory")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProductForCategory(int categoryId)
    {
        var products = _productRepository.GetProductsByCategoryId(categoryId);
        if (products.Count == 0)
        {
            return NotFound($"Los productos con la categoryId {categoryId} no existe");
        }
        
        var productsDto = _mapper.Map<List<ProductDto>>(products);
        return Ok(productsDto);
    }

    [HttpGet("searchProductByNameDescription/{seachTerm}", Name = "GetProductsByNameDescription")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProductsByNameDescription(string seachTerm)
    {
        var products = _productRepository.SearchṔroducts(seachTerm);
        if (products.Count == 0)
        {
            return NotFound($"No se encontaron productos con ese nombre o descripcion: {seachTerm}");
        }
        
        var productDto = _mapper.Map<List<ProductDto>>(products);
        return Ok(productDto);
    }
    
    [HttpPatch("buyProduct/{productName}/{quantity:int}", Name = "BuyProduct")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult BuyProduct(string productName,  int quantity)
    {
        if (string.IsNullOrWhiteSpace(productName) || quantity <= 0)
        {
            return BadRequest($"El nombre o la cantidad no es valida");
        }
        
        var foundProduct = _productRepository.ProductExits(productName);

        if (!foundProduct)
        {
            return NotFound($"El producto {foundProduct} no existe");
        }

        if (!_productRepository.BuyProduct(productName, quantity))
        {
            ModelState.AddModelError("CustomError", $"No se pudo comprar el producto {productName} o la cantidad no es valida");
            return BadRequest(ModelState);
        }
        
        var units = quantity == 1? "unidad" : "unidades";
        return Ok($"Se compro {quantity} {units} del '{productName}' correctamente");
    }
    
    [HttpPut("{productId:int}", Name = "UpdateProduct")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateProduct(int productId,[FromBody] UpdateProductDto updateProductDto)
    {
        if (updateProductDto == null)
        {
            return BadRequest(ModelState);
        }

        if (!_productRepository.ProductExists(productId))
        {
            ModelState.AddModelError("CustomError", "el producto no existe");
            return BadRequest(ModelState);
        }
        
        if (!_categoryRepository.CategoryExists(updateProductDto.CategoryId))
        {
            ModelState.AddModelError("CustomError", $"la categoria con el id {updateProductDto.CategoryId} no existe");
            return BadRequest(ModelState);
        }
        
        var product = _mapper.Map<Product>(updateProductDto);
        product.ProductId = productId;
        if (!_productRepository.UpdateProduct(product))
        {
            ModelState.AddModelError("CustomError", $"Algo salio mal al actualizar el registro {product.Name}");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }

    [HttpDelete("{productId:int}", Name = "DeleteProduct")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult DeleteProduct(int productId)
    {
        if (productId <= 0)
        {
            return BadRequest(ModelState);
        }

        var product = _productRepository.GetById(productId);
        if (product == null)
        {
            return NotFound($"El producto con el id {productId} no existe");
        }

        if (!_productRepository.DeleteProduct(product))
        {
            ModelState.AddModelError("CustomerError", $"Algo salio mal al eliminar el registro {product.Name}");
        }

        return NoContent();
    }
}