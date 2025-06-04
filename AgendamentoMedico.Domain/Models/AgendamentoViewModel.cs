using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Domain.Models
{
    public class AgendamentoViewModel
    {
        public string DataHora { get; set; }
        public string NomeMedico { get; set; }
        public string NomePaciente { get; set; }
        public string Status { get; set; }
    }
}
