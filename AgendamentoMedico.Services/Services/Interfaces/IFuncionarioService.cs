using AgendamentoMedico.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Services.Services.Interfaces
{
    public interface IFuncionarioService
    {
        Task<IEnumerable<FuncionarioViewModel>> ObterTodosAsync();
        Task<FuncionarioViewModel?> ObterPorIdAsync(Guid id);
        Task CriarAsync(UsuarioViewModel usuarioVm, FuncionarioCreateViewModel vm);
        Task AtualizarAsync(Guid id, FuncionarioViewModel vm);
        Task ExcluirAsync(Guid id);
    }
}
