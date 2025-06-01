using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Domain.Models
{
    public class HorarioDisponivelViewModel
    {
        public Guid Id { get; set; }
        public string InicioConsulta { get; set; }
        public string FimConsulta { get; set; }

    }
}
