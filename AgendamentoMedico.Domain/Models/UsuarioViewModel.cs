using System.ComponentModel.DataAnnotations;

namespace AgendamentoMedico.Domain.Models
{
    public class UsuarioViewModel
    {
        [Required(ErrorMessage = "O nome de Login é obrigatório"), Display(Name = "Login")]
        public string NomeUsuario { get; set; }

        [Required(ErrorMessage = "O email é obrigatório"), EmailAddress, Display(Name = "E‑mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha precisa ter mais que 8 digitos"), DataType(DataType.Password), MinLength(8)]
        public string Senha { get; set; }
    }
}
