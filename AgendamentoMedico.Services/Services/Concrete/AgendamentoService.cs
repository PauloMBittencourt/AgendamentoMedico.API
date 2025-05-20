using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Infra.Repositories.Interfaces;
using AgendamentoMedico.Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Services.Services.Concrete
{
    public class AgendamentoService : IAgendamentoService
    {
        private readonly IHorarioDisponivelRepository _horarioRepositorie;
        private readonly IAgendamentoRepository _agendamentoRepositorie;

        public AgendamentoService(
            IHorarioDisponivelRepository horarioRepo,
            IAgendamentoRepository agendamentoRepo)
        {
            _horarioRepositorie = horarioRepo;
            _agendamentoRepositorie = agendamentoRepo;
        }

        public async Task MarkAsync(Guid horarioId, Guid clienteId)
        {
            var horario = await _horarioRepositorie.GetByIdAsync(horarioId);
            if (horario == null)
                throw new InvalidOperationException("Horário não encontrado.");
            if (!horario.Disponivel)
                throw new InvalidOperationException("Horário já foi agendado.");

            var agendamento = new Funcionario_Cliente
            {
                HorarioDisponivelId = horarioId,
                ClienteId = clienteId
            };
            await _agendamentoRepositorie.AddAsync(agendamento);

            horario.Disponivel = false;
            await _horarioRepositorie.UpdateAsync(horario);
        }
    }
}
