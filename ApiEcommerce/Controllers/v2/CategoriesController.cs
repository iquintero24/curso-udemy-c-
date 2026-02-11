using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers.v2;

[Route("api/v{version:apiVersion}/[controller]")] // http:localhost:1445/categories
[ApiVersion("2.0")]
[ApiController]
[Authorize(Roles = "Admin")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository; // injection de category repository
    private readonly IMapper _mapper; // injection del mapper 

    // constructor principal
    public CategoriesController(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// endpoint get para obtener todas las categorias
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetCategories()
    {
        var categories = _categoryRepository.GetCategories().OrderByDescending(c => c.Id).ToList();
        var categoriesDto = new List<CategoryDto>();
        foreach (var category in categories)
        {
            categoriesDto.Add(_mapper.Map<CategoryDto>(category));
        }

        return Ok(categoriesDto);
    }
    
    /// <summary>
    /// endpoint para obtener una categoria por id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}", Name = "GetCategory")] // string template
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    //[ResponseCache(Duration = 10)]
    [ResponseCache(CacheProfileName = "Default10")]
    [AllowAnonymous]
    public IActionResult GetCategory(int id)
    {
        System.Console.WriteLine($"Categoria con el id {id} a las {DateTime.Now}");
        var category = _categoryRepository.GetCategoryById(id);
        System.Console.WriteLine($"Respuesta con el id {id}");
        if (category == null)
        {
            return NotFound($"La categoria con el id {id} no existe");
        }

        var categoryDto = _mapper.Map<CategoryDto>(category);
        return Ok(categoryDto);
    }
    
    /// <summary>
    /// endpoint para crear categorias 
    /// </summary>
    /// <param name="createCategoryDto"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
    {
        if (createCategoryDto == null)
        {
            return BadRequest(ModelState);
        }

        if (_categoryRepository.CategoryExists(createCategoryDto.Name))
        {
            ModelState.AddModelError("CustomError", "La categoria existe");
            return BadRequest(ModelState);
        }
        
        var category = _mapper.Map<Category>(createCategoryDto);
        if (!_categoryRepository.CreateCategory(category))
        {
            ModelState.AddModelError("CustomError", $"Algo salio mal al guardar el registro {createCategoryDto.Name}");
            return StatusCode(500, ModelState);
        }
        
        return CreatedAtRoute("GetCategory", new { id = category.Id }, category);
        
    }
    
    /// <summary>
    /// endpoint para actualizar categoria por id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updateCategoryDto"></param>
    /// <returns></returns>
    [HttpPatch("{id:int}", Name = "UpdateCategory")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public IActionResult UpdateCategory(int id, [FromBody] CreateCategoryDto updateCategoryDto)
    {
        if (!_categoryRepository.CategoryExists(id))
        {
            return NotFound($"La categoria con el id {id} no existe");
        }

        if (updateCategoryDto == null)
        {
            return BadRequest(ModelState);
        }

        if (_categoryRepository.CategoryExists(updateCategoryDto.Name))
        {
            ModelState.AddModelError("CustomError", "La categoria existe");
            return BadRequest(ModelState);
        }

        var category = _mapper.Map<Category>(updateCategoryDto);
        category.Id = id;
        if (!_categoryRepository.UpdateCategory(category))
        {
            ModelState.AddModelError("CustomError", $"Algo salio mal al actualizar el registro {category.Name}");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
    
    /// <summary>
    /// endpoint para eliminar categoria 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:int}", Name = "DeleteCategory")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult DeleteCategory(int id)
    {
        if (!_categoryRepository.CategoryExists(id))
        {
            return NotFound($"La categoria con el id {id} no existe");
        }

        var category = _categoryRepository.GetCategoryById(id);
     
        if (category == null)
        {
            return NotFound($"La categoria con el id {id} no existe");
        }

        if (!_categoryRepository.DeleteCategory(category))
        {
            ModelState.AddModelError("CustomError", $"Algo salio mal al eliminar el registro {category.Name}");
            return StatusCode(500, ModelState);
        }
        
        return NoContent();
    }

}