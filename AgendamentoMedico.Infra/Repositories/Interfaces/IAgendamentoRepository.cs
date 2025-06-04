using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Infra.Repositories.Interfaces
{
    public interface IAgendamentoRepository
    {
        Task AddAsync(Funcionario_Cliente agendamento);
        List<Funcionario_Cliente> GetAgendamentosByClientId(Guid pacienteId);
        List<Funcionario_Cliente> GetAgendamentosByMedicId(Guid funcionarioId);
    }
}
