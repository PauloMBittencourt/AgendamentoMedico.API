using AgendamentoMedico.Domain.Entities;

namespace AgendamentoMedico.Domain.Models
{
    public class CreateViewModel
    {
        public Funcionario funcionario { get; set; }
        public Usuario Usuario { get; set; }
    }
}
