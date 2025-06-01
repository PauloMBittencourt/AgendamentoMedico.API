using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AgendamentoMedico.Domain.Models
{
    public class LoginViewModel
    {
        [DisplayName("Login do Usuário")]
        [Required(ErrorMessage = "Informe o nome do usuário", AllowEmptyStrings = false)]
        public string NomeUsuario { get; set; }
        [Required(ErrorMessage = "Informe a senha do usuário", AllowEmptyStrings = false)]
        public string Senha { get; set; }
        [DisplayName("Lembrar senha?")]
        public bool RememberMe { get; set; }
    }
}
