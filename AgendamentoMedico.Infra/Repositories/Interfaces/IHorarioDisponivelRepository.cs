using AgendamentoMedico.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Infra.Repositories.Interfaces
{
    public interface IHorarioDisponivelRepository
    {
        Task<IEnumerable<HorarioDisponivel>> GetByFuncionarioAsync(Guid funcionarioId);
        Task<IEnumerable<HorarioDisponivel>> GetDisponiveisAsync();
        Task<HorarioDisponivel?> GetByIdAsync(Guid id);
        Task AddAsync(HorarioDisponivel horario);
        Task UpdateAsync(HorarioDisponivel horario);
    }
}
