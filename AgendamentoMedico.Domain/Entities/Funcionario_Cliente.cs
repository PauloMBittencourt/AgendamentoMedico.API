using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Domain.Entities
{
    [Table("Agendamentos")]
    public class Funcionario_Cliente
    {
        [Key]
        [Column(Order = 0)]
        public Guid ClienteId { get; set; }
        [Key]
        [Column(Order = 1)]
        public Guid HorarioDisponivelId { get; set; }

        [ForeignKey(nameof(ClienteId))]
        public Cliente ClienteFk { get; set; }

        [ForeignKey(nameof(HorarioDisponivelId))]
        public HorarioDisponivel HorarioDisponivel { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public DateTime DataAgendamento { get; set; }

    }
}
