using System.ComponentModel.DataAnnotations;

namespace Cinema.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O Nome é obrigatório")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "O Sobrenome é obrigatório")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "O Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "O CPF é obrigatório")]
        public string CPF { get; set; } = null!;

        [Required(ErrorMessage = "A Senha é obrigatória")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
        public string Password { get; set; } = null!;

        [Compare("Password", ErrorMessage = "As senhas não conferem")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
