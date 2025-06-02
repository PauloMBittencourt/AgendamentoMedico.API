using AgendamentoMedico.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Services.Services.Interfaces
{
    public interface ICargosService
    {
        Task<bool> CargoDelete(IdentityRole cargo);
        Task<bool> CargoSalvar(IdentityRole cargo);
        Task<IdentityRole> ObterCargoPorDesc(string Cargo);
        List<string> ObterCargosUsuarioPorIdAsync(Guid Id);
        List<IdentityRole_Usuario> ObterCargosUsuariosPorId(Guid UserId);
        Task<List<IdentityRole>> ObterTodosCargos();
        Task<List<string>> ObterTodosCargosDescAsync(bool incluirAdm = false);
        Task<IdentityRole_Usuario> ObterUsuarioComCargosPorId(Guid usuarioId, Guid cargoId);
        Task<bool> SalvarListaCargos_Usuario(List<IdentityRole_Usuario> roles);
        Task<bool> SalvarCargos_Usuario(IdentityRole_Usuario roles);
        Task<bool> RemoverCargos_Usuario(IdentityRole_Usuario roles);
    }
}
