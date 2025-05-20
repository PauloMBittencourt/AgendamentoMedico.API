using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Services.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario?> ObterPorIdAsync(Guid id);
        Task<Usuario?> ObterPorNomeAsync(string nomeUsuario);
        Task CriarAsync(UsuarioViewModel vm);
        Task AtualizarAsync(Guid id, UsuarioViewModel vm);
        Task ExcluirAsync(Guid id);
        Task<IEnumerable<Usuario>> ObterTodosAsync();
        Task<Usuario> ObterUsuarioPorSenha(string nomeUsuario, string senha);
    }
}
