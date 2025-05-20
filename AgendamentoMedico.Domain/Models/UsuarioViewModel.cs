using System.ComponentModel.DataAnnotations;

namespace AgendamentoMedico.Domain.Models
{
    public class UsuarioViewModel
    {
        [Required, Display(Name = "Login")]
        public string NomeUsuario { get; set; }

        [Required, DataType(DataType.Password), MinLength(8)]
        public string Senha { get; set; }
    }
}
