using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models.Dtos
{
    public class LoginUserDto
    {

        [Required(ErrorMessage = "El campo username es requerido")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "El campo password es requerido")]
        public string? Password { get; set; }

    }
}
