using AgendamentoMedico.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Infra.Repositories.Interfaces
{
    public interface ICargosRepository
    {
        Task<List<IdentityRole>> GetAllRoles();
        Task<List<string>> GetAllRolesDescAsync(bool admIn = false);
        Task<IdentityRole> GetRoleByDesc(string Cargo);
        IQueryable<string> GetRoles_UserByIdAsync(Guid Id);
        List<IdentityRole_Usuario> GetRoles_UsersById(Guid UserId);
        Task<IdentityRole_Usuario> GetRole_UserSameById(Guid usuarioId, Guid cargoId);
        Task<bool> RoleAdd(IdentityRole cargo);
        Task<bool> RoleDelete(IdentityRole cargo);
        Task<bool> RolesUserSave(List<IdentityRole_Usuario> roles);
        Task<bool> RolesUserSave(IdentityRole_Usuario roles);
        Task<bool> RoleUpdate(IdentityRole cargo);
        Task<bool> DeleteRole_User(IdentityRole_Usuario cargo);
    }
}
