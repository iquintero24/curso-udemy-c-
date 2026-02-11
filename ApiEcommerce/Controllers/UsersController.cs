using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/v{version:apiVersion}/[controller]")]  // http:localhost:1445/Users
[ApiController]
[ApiVersionNeutral]
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
    [Authorize]
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

    [AllowAnonymous]
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
    
    [AllowAnonymous]
    [HttpPost("login", Name = "LoginUser")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserDto loginUserDto)
    {
        if (loginUserDto == null || !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var user = await _userRepository.Login(loginUserDto);
        if (user == null)
        {
            return Unauthorized();
        }
        
        return Ok(user);
    }
    
}