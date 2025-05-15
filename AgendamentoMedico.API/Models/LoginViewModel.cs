using System.ComponentModel.DataAnnotations;

namespace AgendamentoMedico.API.Models
{
    public class LoginViewModel
    {
        public string NomeUsuario { get; set; }
        public string Senha { get; set; }
    }
}
