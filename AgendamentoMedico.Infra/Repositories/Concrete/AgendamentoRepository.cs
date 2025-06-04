using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Domain.Models;
using AgendamentoMedico.Infra.Data;
using AgendamentoMedico.Infra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgendamentoMedico.Infra.Repositories.Concrete
{
    public class AgendamentoRepository : IAgendamentoRepository
    {
        private readonly AppDbContext _context;

        public AgendamentoRepository(AppDbContext context)
            => _context = context;

        public async Task AddAsync(Funcionario_Cliente agendamento)
        {
            _context.Agendamentos.Add(agendamento);
            await _context.SaveChangesAsync();
        }

        public List<Funcionario_Cliente> GetAgendamentosByMedicId(Guid funcionarioId)
        {
            return _context.Agendamentos
                .Include(fc => fc.HorarioDisponivel)
                .ThenInclude(h => h.Funcionario)
                .Include(fc => fc.ClienteFk)
                .Where(fc => fc.HorarioDisponivel.FuncionarioId == funcionarioId)
                .OrderBy(fc => fc.HorarioDisponivel.DataHora)
                .ToList();
        }

        public List<Funcionario_Cliente> GetAgendamentosByClientId(Guid pacienteId)
        {
            return _context.Agendamentos
                .Include(fc => fc.HorarioDisponivel)
                .ThenInclude(h => h.Funcionario)
                .Include(fc => fc.ClienteFk)
                .Where(fc => fc.ClienteId == pacienteId)
                .AsNoTracking()
                .OrderBy(fc => fc.HorarioDisponivel.DataHora)
                .ToList();
        }

    }
}
