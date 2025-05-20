using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Infra.Data;
using AgendamentoMedico.Infra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Infra.Repositories.Concrete
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
            => _context = context;

        public async Task<List<Usuario>> GetAllAsync() =>
            await _context.Usuarios
            .Include(c => c.ClienteId)
            .Include(f => f.FuncionarioId)
            .Include(r => r.Cargos)
            .ToListAsync();

        public async Task<Usuario?> GetByIdAsync(Guid id)
            => await _context.Usuarios
                             .AsNoTracking()
                             .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<Usuario?> GetByUsernameAsync(string nomeUsuario)
            => await _context.Usuarios
                             .AsNoTracking()
                             .FirstOrDefaultAsync(u => u.NomeUsuario == nomeUsuario);

        public async Task AddAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = new Usuario { Id = id };
            _context.Usuarios.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
