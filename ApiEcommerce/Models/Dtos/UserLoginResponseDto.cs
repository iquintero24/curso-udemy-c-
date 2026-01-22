namespace ApiEcommerce.Models.Dtos
{
    public class UserLoginResponseDto
    {
        public RegisterUserDto? User { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
    }
}
