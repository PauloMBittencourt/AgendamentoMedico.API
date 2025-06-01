using System.ComponentModel.DataAnnotations;

namespace AgendamentoMedico.Domain.Models
{
    public class UsuarioViewModel
    {
        [Required, Display(Name = "Login")]
        public string NomeUsuario { get; set; }

        [Required, EmailAddress, Display(Name = "E‑mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha precisa ter mais que 8 digitos"), DataType(DataType.Password), MinLength(8)]
        public string Senha { get; set; }
    }
}
