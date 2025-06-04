using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Infra.Data;
using AgendamentoMedico.Infra.Repositories.Interfaces;
using AgendamentoMedico.Utils.Encrypt;
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
        private static readonly Guid ClienteRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

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
                             .Include(f => f.FuncionarioId)
                             .Include(f => f.ClienteId)
                             .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<Usuario?> GetByUsernameAsync(string nomeUsuario)
            => await _context.Usuarios
                             .AsNoTracking()
                             .Include(c => c.Cargos)
                             .ThenInclude(cf => cf.CargosIdentityFk)
                             .Include(f => f.FuncionarioId)
                             .Include(cl => cl.ClienteId)
                             .FirstOrDefaultAsync(u => u.NomeUsuario == nomeUsuario);

        public async Task<Usuario?> GetByUsernameAndPasswordAsync(string nomeUsuario, string senha)
        {
            var user = await _context.Usuarios
                    .Include(f => f.Cargos)
                    .ThenInclude(pf => pf.CargosIdentityFk)
                    .Include(f => f.FuncionarioId)
                    .FirstOrDefaultAsync(f =>
                        f.NomeUsuario == nomeUsuario);
            
            if (EncryptUtils.DecryptPasswordBase64(EncryptUtils.DecryptPassword(user.Senha)) != senha)
            {
                return null;
            }

            return user;
        }

        public async Task AddAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);

            var cargoUsuario = new IdentityRole_Usuario
            {
                UsuarioId = usuario.Id,
                CargosIdentityId = ClienteRoleId
            };
            _context.CargosIdentity_Usuarios.Add(cargoUsuario);

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
