using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Services.Services.Interfaces
{
    public interface IAgendamentoService
    {
        Task MarkAsync(Guid horarioId, Guid clienteId);
    }
}
