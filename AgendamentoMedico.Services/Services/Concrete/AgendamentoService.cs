using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Domain.Models;
using AgendamentoMedico.Infra.Repositories.Concrete;
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
        private readonly IHorarioDisponivelRepository _horarioRepository;
        private readonly IAgendamentoRepository _agendamentoRepository;

        public AgendamentoService(IHorarioDisponivelRepository horarioRepository, IAgendamentoRepository agendamentoRepository)
        {
            _horarioRepository = horarioRepository;
            _agendamentoRepository = agendamentoRepository;
        }

        public async Task AgendarConsultaAsync(Guid horarioId, Guid clienteId)
        {
            var horario = await _horarioRepository.GetByIdAsync(horarioId);
            if (horario == null)
                throw new InvalidOperationException("Horário não encontrado.");
            if (!horario.Disponivel)
                throw new InvalidOperationException("Horário já foi agendado.");

            var agendamento = new Funcionario_Cliente
            {
                HorarioDisponivelId = horarioId,
                ClienteId = clienteId,
                DataAgendamento = DateTime.Now
            };

            await _agendamentoRepository.AddAsync(agendamento);

            horario.Disponivel = false;
            await _horarioRepository.UpdateAsync(horario);
        }

        public List<AgendamentoViewModel> ObterAgendamentosDoMedico(Guid funcionarioId)
        {
            var agendamentos = _agendamentoRepository.GetAgendamentosByMedicId(funcionarioId);

            return agendamentos.Select(fc => new AgendamentoViewModel
            {
                DataHora = fc.HorarioDisponivel.DataHora.ToString("s"),
                NomePaciente = fc.ClienteFk.Nome,
                Status = "Confirmado"
            }).ToList();
        }

        public List<AgendamentoViewModel> ObterAgendamentosDoPaciente(Guid pacienteId)
        {
            var agendamentos = _agendamentoRepository.GetAgendamentosByClientId(pacienteId);

            return agendamentos.Select(fc => new AgendamentoViewModel
            {
                DataHora = fc.HorarioDisponivel.DataHora.ToString("s"),
                NomeMedico = fc.HorarioDisponivel.Funcionario.Nome,
                Status = "Confirmado"
            }).ToList();
        }
    }
}
