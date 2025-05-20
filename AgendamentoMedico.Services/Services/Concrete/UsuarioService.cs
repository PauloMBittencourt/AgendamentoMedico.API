using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Domain.Models;
using AgendamentoMedico.Infra.Repositories.Interfaces;
using AgendamentoMedico.Services.Services.Interfaces;
using AgendamentoMedico.Utils.Encrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Services.Services.Concrete
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repo;

        public UsuarioService(IUsuarioRepository repo)
            => _repo = repo;

        public async Task<Usuario?> ObterPorIdAsync(Guid id)
            => await _repo.GetByIdAsync(id);

        public async Task<Usuario?> ObterPorNomeAsync(string nomeUsuario)
            => await _repo.GetByUsernameAsync(nomeUsuario);

        public async Task<IEnumerable<Usuario>> ObterTodosAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(c => new Usuario
            {
                Id = c.Id,
                Cargos = c.Cargos,
                NomeUsuario = c.NomeUsuario,
                FuncionarioId = c.FuncionarioId,
                ClienteId = c.ClienteId
            });
        }

        public async Task CriarAsync(UsuarioViewModel vm)
        {
            var base64 = EncryptUtils.EncryptPasswordBase64(vm.Senha);
            var cifrado = EncryptUtils.EncryptPassword(base64);

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                NomeUsuario = vm.NomeUsuario,
                Senha = cifrado
            };

            await _repo.AddAsync(usuario);
        }

        public async Task AtualizarAsync(Guid id, UsuarioViewModel vm)
        {
            var existente = await _repo.GetByIdAsync(id);
            if (existente == null) throw new InvalidOperationException("Usuário não encontrado");

            existente.NomeUsuario = vm.NomeUsuario;

            if (!string.IsNullOrWhiteSpace(vm.Senha))
            {
                var base64 = EncryptUtils.EncryptPasswordBase64(vm.Senha);
                existente.Senha = EncryptUtils.EncryptPassword(base64);
            }

            await _repo.UpdateAsync(existente);
        }

        public async Task ExcluirAsync(Guid id)
        {
            await _repo.DeleteAsync(id);
        }

        public async Task<Usuario> ObterUsuarioPorSenha(string nomeUsuario, string senha)
        {
            var usuario = await _repo.GetByUsernameAndPasswordAsync(nomeUsuario, senha);
            return usuario;
        }
    }
}
