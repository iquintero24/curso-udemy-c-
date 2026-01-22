using ApiEcommerce.Models.Dtos;

namespace ApiEcommerce.Repository.IRepository;

public interface IUserRepository
{
    ICollection<User> GetUsers();
    User? GetUser(int id);
    bool IsUniqueUser(string username);
    Task<UserLoginResponseDto>Login(LoginUserDto loginUserDto);
    Task<User> RegisterUser(CreateUserDto createUserDto);
}