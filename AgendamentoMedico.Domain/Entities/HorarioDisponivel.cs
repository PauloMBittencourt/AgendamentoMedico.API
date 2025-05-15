using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Domain.Entities
{
    [Table("HorariosDisponiveis")]
    public class HorarioDisponivel
    {
        [Key]
        public Guid HorarioDisponivelId { get; set; }
        public Guid FuncionarioId { get; set; }
        public DateTime DataHora { get; set; }
        public bool Disponivel { get; set; } = true;


        [ForeignKey(nameof(FuncionarioId))]
        public Funcionario Funcionario { get; set; }
        public ICollection<Funcionario_Cliente> Agendamentos { get; set; }
    }

}
