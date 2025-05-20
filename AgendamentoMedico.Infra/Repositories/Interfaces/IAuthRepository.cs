using AgendamentoMedico.Domain.Entities;

namespace AgendamentoMedico.Infra.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<Usuario?> GetByUsernameAsync(string nomeUsuario);
    }
}
