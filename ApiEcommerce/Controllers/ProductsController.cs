using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
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
        
        return CreatedAtRoute("GetProduct", new { productId = product.ProductId  }, product); 
        
    }

}