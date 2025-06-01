using System.ComponentModel.DataAnnotations;

namespace AgendamentoMedico.Domain.Models
{
    public class ClienteViewModel
    {
        [Required, Display(Name = "Nome Completo")]
        public string Nome { get; set; }

        [Phone]
        public string Telefone { get; set; }
    }
}
