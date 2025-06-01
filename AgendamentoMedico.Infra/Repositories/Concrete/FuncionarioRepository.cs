using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Domain.Enuns;
using AgendamentoMedico.Infra.Data;
using AgendamentoMedico.Infra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgendamentoMedico.Infra.Repositories.Concrete
{
    public class FuncionarioRepository : IFuncionarioRepository
    {
        private readonly AppDbContext _context;

        public FuncionarioRepository(AppDbContext context)
            => _context = context;

        public async Task<IEnumerable<Funcionario>> GetAllAsync()
        {
            return await _context.Funcionarios
                                 .Include(f => f.UsuarioFuncionario)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Funcionario>> GetAllDoctors()
        {
            return await _context.Funcionarios
                     .Include(f => f.UsuarioFuncionario)
                     .Where(d => d.Cargo == EnumCargo.Medico)
                     .ToListAsync();
        }

        public async Task<Funcionario?> GetByIdAsync(Guid id)
        {
            return await _context.Funcionarios
                                 .Include(f => f.UsuarioFuncionario)
                                 .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task AddAsync(Funcionario funcionario)
        {
            _context.Funcionarios.Add(funcionario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Funcionario funcionario)
        {
            _context.Funcionarios.Update(funcionario);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = new Funcionario { Id = id };
            _context.Funcionarios.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
