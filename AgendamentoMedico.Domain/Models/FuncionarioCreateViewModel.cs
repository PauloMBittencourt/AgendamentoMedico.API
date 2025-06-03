using Microsoft.AspNetCore.Mvc.Rendering;

namespace AgendamentoMedico.Domain.Models
{
    public class FuncionarioCreateViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public Guid Cargo { get; set; }
        public IEnumerable<SelectListItem> CargosDisponiveis { get; set; }
    }
}
