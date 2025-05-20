using AgendamentoMedico.Domain.Models;
using AgendamentoMedico.Infra.Repositories.Interfaces;
using AgendamentoMedico.Services.Services.Interfaces;
using AgendamentoMedico.Utils.Encrypt;

namespace AgendamentoMedico.Services.Services.Concrete
{
    public class AuthServices : IAuthServices
    {
        private readonly IAuthRepository _repo;

        public AuthServices(IAuthRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> ValidateCredentialsAsync(LoginViewModel login)
        {
            var user = await _repo.GetByUsernameAsync(login.NomeUsuario);

            if (user == null) return false;

            var aesDecrypted = EncryptUtils.DecryptPassword(user.Senha);
            var originalPassword = EncryptUtils.DecryptPasswordBase64(aesDecrypted);

            return originalPassword == login.Senha;
        }
    }
}
