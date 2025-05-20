using AgendamentoMedico.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Services.Services.Interfaces
{
    public interface IHorarioDisponivelService
    {
        Task<IEnumerable<HorarioDisponivel>> ObterTodosPorFuncionarioAsync(Guid funcionarioId);
        Task<IEnumerable<HorarioDisponivel>> ObterDisponiveisAsync();
        Task CriarAsync(HorarioDisponivel horario);
    }
}
