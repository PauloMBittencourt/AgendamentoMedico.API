using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Services.Services.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<Cliente>> ObterTodosAsync();
        Task<Cliente?> ObterPorIdAsync(Guid? id);
        Task CriarAsync(UsuarioViewModel usuarioVm, ClienteViewModel clienteVm);
        Task AtualizarAsync(ClienteViewModel vm);
        Task ExcluirAsync(Guid? id);
    }
}
