using System.ComponentModel.DataAnnotations;

namespace AgendamentoMedico.Domain.Models
{
    public class LoginViewModel
    {
        public string NomeUsuario { get; set; }
        public string Senha { get; set; }
    }
}
