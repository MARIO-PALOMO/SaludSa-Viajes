using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Usuario")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }
}
