using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers;

[Route("api/[controller]")] // http:localhost:1445/Users
[ApiController]
public class UsersController: ControllerBase
{
    // injection de dependencias
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetUsers()
    {
        var users = _userRepository.GetUsers();
        var usersDto = _mapper.Map<List<UserDto>>(users);
        return Ok(usersDto);
    }

    [HttpGet("{userId:int}", Name = "GetUser")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetUserById(int userId)
    {
        var user = _userRepository.GetUser(userId);
        if (user == null)
        {
            return NotFound($"El Usuario con id {userId} no existe");
        }
        
        var userDto = _mapper.Map<UserDto>(user);
        return Ok(userDto);
        
    }

    [HttpPost(Name = "RegisterUser")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> registerUser([FromBody] CreateUserDto createUserDto)
    {
        if (createUserDto == null || !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (string.IsNullOrWhiteSpace(createUserDto.Username))
        {
            return BadRequest("Usernam es requerido");
        }

        if (!_userRepository.IsUniqueUser(createUserDto.Username))
        {
            return BadRequest("El usuario ya esta registrado");
        }

        var result = await _userRepository.RegisterUser(createUserDto);
        if (result == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error al registrar un usuario");
        }
        return CreatedAtRoute("GetUser", new { userId = result.Id }, result);
    }
}