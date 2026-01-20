namespace ApiEcommerce.Models.Dtos
{
    public class RegisterUserDto
    {

        public string? ID { get; set; }

        public string? Name { get; set; }

        public required string UserName { get; set; }

        public required string Password { get; set; }

        public string? Rol { get; set; }

    }
}
