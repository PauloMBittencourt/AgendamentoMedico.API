using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Domain.Models;
using AgendamentoMedico.Infra.Data;
using AgendamentoMedico.Infra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgendamentoMedico.Infra.Repositories.Concrete
{
    public class HorarioDisponivelRepository : IHorarioDisponivelRepository
    {
        private readonly AppDbContext _context;

        public HorarioDisponivelRepository(AppDbContext context)
            => _context = context;

        public async Task<IEnumerable<HorarioDisponivel>> GetByFuncionarioAsync(Guid funcionarioId)
        {
            return await _context.HorariosDisponiveis
                                 .Include(f => f.Funcionario)
                                 .Where(h => h.FuncionarioId == funcionarioId)
                                 .OrderBy(h => h.DataHora)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<IEnumerable<HorarioDisponivel>> GetDisponiveisAsync()
        {
            return await _context.HorariosDisponiveis
                                 .Where(h => h.Disponivel)
                                 .OrderBy(h => h.DataHora)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public List<HorarioDisponivel> GetDisponiveisDoctor(Guid funcionarioId)
        {
            return _context.HorariosDisponiveis
                .Where(h => h.FuncionarioId == funcionarioId && h.Disponivel)
                .AsNoTracking()
                .OrderBy(h => h.DataHora)
                .ToList();
        }


        public async Task<HorarioDisponivel?> GetByIdAsync(Guid id)
        {
            return await _context.HorariosDisponiveis
                                 .FirstOrDefaultAsync(h => h.HorarioDisponivelId == id);
        }

        public async Task AddAsync(HorarioDisponivel horario)
        {
            _context.HorariosDisponiveis.Add(horario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(HorarioDisponivel horario)
        {
            _context.HorariosDisponiveis.Update(horario);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Guid id)
        {
            var horario = await GetByIdAsync(id);
            if (horario != null)
            {
                _context.HorariosDisponiveis.Remove(horario);
                await _context.SaveChangesAsync();
            }
        }


    }
}
