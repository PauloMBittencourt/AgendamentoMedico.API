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
    public class CargosRepository : ICargosRepository
    {
        private readonly AppDbContext _context;
        public CargosRepository(AppDbContext context)
            => _context = context;
        public async Task<bool> RolesUserSave(List<IdentityRole_Usuario> roles)
        {
            _context.RemoveRange(roles);
            await _context.SaveChangesAsync();
            return true;
        }

        public IQueryable<string> GetAllRolesDescAsync(bool admIn = false)
        {
            IQueryable<string> cargo;

            if (admIn)
            {
                cargo = _context.CargosIdentity
                    .Select(p => p.Descricao)
                    .AsQueryable();

                return cargo;
            }

            cargo = _context.CargosIdentity
                .Where(p => p.Descricao != "Administrador")
                .Select(p => p.Descricao);

            return cargo;
        }

        public async Task<List<IdentityRole>> GetAllRoles()
        {
            var lista = await _context.CargosIdentity.ToListAsync();
            return lista;
        }

        public async Task<bool> RolesUserSave(IdentityRole_Usuario roles)
        {
            _context.CargosIdentity_Usuarios.Add(roles);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IdentityRole> GetRoleByDesc(string Cargo)
        {
            var cargo = await _context.CargosIdentity.Where(p => p.Descricao == Cargo).FirstOrDefaultAsync();

            return cargo;
        }
        public async Task<bool> RoleAdd(IdentityRole cargo)
        {
            _context.CargosIdentity.Add(cargo);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RoleUpdate(IdentityRole cargo)
        {
            _context.CargosIdentity.Update(cargo);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RoleDelete(IdentityRole cargo)
        {
            _context.CargosIdentity.Remove(cargo);
            await _context.SaveChangesAsync();
            return true;
        }

        #region Metódos do CagosIdentity_Usuarios
        public IQueryable<string> GetRoles_UserByIdAsync(Guid Id)
        {
            var cargo = _context.CargosIdentity_Usuarios
                .Include(pf => pf.CargosIdentityFk)
                .Where(pf => pf.UsuarioId == Id)
                .Select(pf => pf.CargosIdentityFk.Descricao);

            return cargo;
        }

        public List<IdentityRole_Usuario> GetRoles_UsersById(Guid UserId)
        {
            var listaCargosusuarios = _context.CargosIdentity_Usuarios
                .Where(pf => pf.UsuarioId == UserId && pf.CargosIdentityFk.Descricao != "Administrador")
                .ToList();

            return listaCargosusuarios;
        }

        public async Task<IdentityRole_Usuario> GetRole_UserSameById(Guid usuarioId, Guid cargoId)
        {
            var cargoUsuario = await _context.CargosIdentity_Usuarios
                .Where(pf => pf.UsuarioId == usuarioId && pf.CargosIdentityId == cargoId)
                .FirstOrDefaultAsync();
            return cargoUsuario;
        }

        public async Task<bool> DeleteRole_User(IdentityRole_Usuario cargo)
        {
            _context.CargosIdentity_Usuarios.Remove(cargo);
            await _context.SaveChangesAsync();
            return true;
        }
        #endregion
    }
}
