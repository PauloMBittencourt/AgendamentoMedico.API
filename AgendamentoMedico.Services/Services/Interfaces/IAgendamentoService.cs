using AgendamentoMedico.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Services.Services.Interfaces
{
    public interface IAgendamentoService
    {
        Task AgendarConsultaAsync(Guid horarioId, Guid clienteId);
        List<AgendamentoViewModel> ObterAgendamentosDoMedico(Guid funcionarioId);
        List<AgendamentoViewModel> ObterAgendamentosDoPaciente(Guid pacienteId);
    }
}
