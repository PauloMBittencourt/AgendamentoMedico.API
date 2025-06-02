using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Domain.Models;
using AgendamentoMedico.Infra.Repositories.Interfaces;
using AgendamentoMedico.Services.Services.Interfaces;

namespace AgendamentoMedico.Services.Services.Concrete
{
    public class HorarioDisponivelService : IHorarioDisponivelService
    {
        private readonly IHorarioDisponivelRepository _repo;

        public HorarioDisponivelService(IHorarioDisponivelRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<HorarioDisponivel>> ObterTodosPorFuncionarioAsync(Guid funcionarioId)
        {
            return await _repo.GetByFuncionarioAsync(funcionarioId);
        }

        public async Task<IEnumerable<HorarioDisponivel>> ObterDisponiveisAsync()
        {
            return await _repo.GetDisponiveisAsync();
        }

        public async Task<IEnumerable<HorarioDisponivelViewModel>> ObterDisponiveisMedicoAsync(Funcionario medico)
        {
            return await _repo.GetDisponiveisDoctorAsync(medico);
        }

        public async Task<HorarioDisponivel> ObterPorId(Guid id)
        {
            var horario = await _repo.GetByIdAsync(id);
            return horario ?? throw new KeyNotFoundException("Horário não encontrado.");
        }

        public async Task CriarAsync(HorarioDisponivel horario)
        {
            if (horario.DataHora <= DateTime.Now)
                throw new ArgumentException("Data e hora devem ser futuras.");

            horario.HorarioDisponivelId = Guid.NewGuid();
            horario.Disponivel = true;
            await _repo.AddAsync(horario);
        }

        public async Task Remover(Guid id)
        {
            await _repo.RemoveAsync(id);
        }
    }
}
