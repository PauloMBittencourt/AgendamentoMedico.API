using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Domain.Models;
using AgendamentoMedico.Infra.Repositories.Interfaces;
using AgendamentoMedico.Services.Services.Interfaces;
using AgendamentoMedico.Utils.Encrypt;

namespace AgendamentoMedico.Services.Services.Concrete
{
    public class FuncionarioService : IFuncionarioService
    {
        private readonly IFuncionarioRepository _repo;
        private readonly IUsuarioRepository _usuarioRepo;

        public FuncionarioService(IFuncionarioRepository repo, IUsuarioRepository usuarioRepo)
        {
            _repo = repo;
            _usuarioRepo = usuarioRepo;
        }

        public async Task<IEnumerable<FuncionarioViewModel>> ObterTodosAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(f => new FuncionarioViewModel
            {
                Id = f.Id,
                Nome = f.Nome,
                Email = f.Email,
                NomeUsuario = f.UsuarioFuncionario?.NomeUsuario
            });
        }

        public async Task<FuncionarioViewModel?> ObterPorIdAsync(Guid id)
        {
            var f = await _repo.GetByIdAsync(id);
            if (f == null) return null;
            return new FuncionarioViewModel
            {
                Id = f.Id,
                Nome = f.Nome,
                Email = f.Email,
                NomeUsuario = f.UsuarioFuncionario?.NomeUsuario
            };
        }

        public async Task CriarAsync(UsuarioViewModel usuarioVm, FuncionarioCreateViewModel vm)
        {
            var senha = EncryptUtils.EncryptPasswordBase64(usuarioVm.Senha);
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                NomeUsuario = usuarioVm.NomeUsuario,
                Senha = EncryptUtils.EncryptPassword(senha),
                Email = usuarioVm.Email
            };
            await _usuarioRepo.AddAsync(usuario);

            var entity = new Funcionario
            {
                Id = Guid.NewGuid(),
                Nome = vm.Nome,
                Email = vm.Email,
                UsuarioFuncionario = usuario
            };
            await _repo.AddAsync(entity);
        }

        public async Task AtualizarAsync(Guid id, FuncionarioViewModel vm)
        {
            var existing = new Funcionario
            {
                Id = id,
                Nome = vm.Nome,
                Email = vm.Email,
                UsuarioId = vm.UsuarioId
            };
            await _repo.UpdateAsync(existing);
        }

        public async Task ExcluirAsync(Guid id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
