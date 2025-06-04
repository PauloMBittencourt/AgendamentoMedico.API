using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AgendamentoMedico.Domain.Models
{
    public class CargoViewModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public string Descricao { get; set; }
    }
}
