using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Infra.Data;
using AgendamentoMedico.Infra.Repositories.Interfaces;

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
    }
}
