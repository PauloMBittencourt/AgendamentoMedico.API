using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AgendamentoMedico.Domain.Models
{
    public class FuncionarioCreateViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um email válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Selecione um Cargo.")]
        public string Cargo { get; set; }

        public IEnumerable<SelectListItem> CargosDisponiveis { get; set; }
    }
}
