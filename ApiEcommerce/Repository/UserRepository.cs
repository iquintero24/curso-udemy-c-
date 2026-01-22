using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiEcommerce.Data;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ApiEcommerce.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;
    private string? secretKey;

    public UserRepository(ApplicationDbContext dbContext , IConfiguration configuration)
    {
        _dbContext = dbContext;
        secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
    }

    public ICollection<User> GetUsers()
    {
        return _dbContext.Users.OrderBy(u => u.UserName).ToList();
    }

    public User? GetUser(int id)
    {
        return _dbContext.Users.FirstOrDefault(u => u.Id == id);
    }

    public bool IsUniqueUser(string username)
    {
        return !_dbContext.Users.Any(u => u.UserName.ToLower().Trim() == username.ToLower().Trim());
    }

    public async Task<UserLoginResponseDto> Login(LoginUserDto loginUserDto)
    {
        if (string.IsNullOrEmpty(loginUserDto.Username))
        {
            return new UserLoginResponseDto
            {
                Token = "",
                User = null,
                Message =  "Username is Required"
            };
        }
        
        var user = await _dbContext.Users.FirstOrDefaultAsync<User>(u=> u.UserName.ToLower().Trim() == loginUserDto.Username.ToLower().Trim());

        if (user == null)
        {
            return new UserLoginResponseDto
            {
                Token = "",
                User = null,
                Message =  "Username no encontrado"
            };
        }

        if (!BCrypt.Net.BCrypt.Verify(loginUserDto.Password, user.Password))
        {
            return new UserLoginResponseDto
            {
                Token = "",
                User = null,
                Message =  "Las credeciales son incorrectas"
            };
        }
        
        // jwt generate
        var handlerToken = new JwtSecurityTokenHandler();
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("La secret Key no esta configurada");
        }
        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.UserName),
                new Claim(ClaimTypes.Role, user.Rol ?? string.Empty),
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = handlerToken.CreateToken(tokenDescriptor);

        return new UserLoginResponseDto()
        {
            Token = handlerToken.WriteToken(token),
            User = new RegisterUserDto()
            {
                UserName = user.UserName,
                Name = user.Name,
                Rol = user.Rol,
                Password = user.Password ?? ""

            },
            Message = "Usuario logueado correctamente"
        };
    }

    public async Task<User> RegisterUser(CreateUserDto createUserDto)
    {
        var encriptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        
        var user = new User
        {
            UserName = createUserDto.Username ?? "No username",
            Name = createUserDto.Name,
            Rol = createUserDto.Role,
            Password = encriptedPassword,
        };
        
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

}