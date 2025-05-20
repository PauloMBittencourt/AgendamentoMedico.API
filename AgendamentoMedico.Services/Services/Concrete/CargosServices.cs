using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Infra.Repositories.Interfaces;
using AgendamentoMedico.Services.Services.Interfaces;
using System.Data;

namespace AgendamentoMedico.Services.Services.Concrete
{
    public class CargosServices : ICargosService
    {
        private readonly ICargosRepository _repo;
        public CargosServices(ICargosRepository repo)
        {
            _repo = repo;
        }

        public List<string> ObterTodosCargosDescAsync(bool incluirAdm = false)
        {
            var cargo = _repo.GetAllRolesDescAsync(incluirAdm).ToList();
            return cargo;
        }

        public List<string> ObterCargosUsuarioPorIdAsync(Guid Id)
        {
            var cargo = _repo.GetRoles_UserByIdAsync(Id).ToList();
            return cargo;
        }

        public async Task<List<IdentityRole>> ObterTodosCargos()
        {
            var lista = await _repo.GetAllRoles();
            return lista;
        }
        public async Task<bool> CargoSalvar(IdentityRole cargo)
        {
            var result = await _repo.RoleAdd(cargo);
            return result;
        }
        public async Task<bool> CargoDelete(IdentityRole cargo)
        {
            var result = await _repo.RoleDelete(cargo);
            return result;
        }
        public async Task<IdentityRole> ObterCargoPorDesc(string Cargo)
        {
            var result = await _repo.GetRoleByDesc(Cargo);
            return result;
        }


        #region Metódos do CagosIdentity_Usuarios
        public async Task<bool> SalvarListaCargos_Usuario(List<IdentityRole_Usuario> roles)
        {
            var result = await _repo.RolesUserSave(roles);
            return result;
        }
        public List<IdentityRole_Usuario> ObterCargosUsuariosPorId(Guid UserId)
        {
            var listaCargosusuarios = _repo.GetRoles_UsersById(UserId);
            return listaCargosusuarios;
        }

        public async Task<bool> SalvarCargos_Usuario(IdentityRole_Usuario roles)
        {
            var result = await _repo.RolesUserSave(roles);
            return result;
        }


        public async Task<IdentityRole_Usuario> ObterUsuarioComCargosPorId(Guid usuarioId, Guid cargoId)
        {
            var cargoUsuario = await _repo.GetRole_UserSameById(usuarioId, cargoId);
            return cargoUsuario;
        }

        public async Task<bool> RemoverCargos_Usuario(IdentityRole_Usuario roles)
        {
            var result = await _repo.DeleteRole_User(roles);
            return result;
        } 
        #endregion
    }
}
