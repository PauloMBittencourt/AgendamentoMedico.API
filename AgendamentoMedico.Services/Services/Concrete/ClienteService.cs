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
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repo;
        private readonly IUsuarioRepository _usuarioRepo;
        public ClienteService(IClienteRepository repo, IUsuarioRepository usuarioRepo)
        {
            _repo = repo;
            _usuarioRepo = usuarioRepo;
        }

        public async Task<IEnumerable<Cliente>> ObterTodosAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(c => new Cliente
            {
                Id = c.Id,
                Nome = c.Nome,
                Email = c.Email,
                Telefone = c.Telefone,
                UsuarioCliente = c.UsuarioCliente
            });
        }

        public async Task<Cliente?> ObterPorIdAsync(Guid? id)
        {
            var c = await _repo.GetByIdAsync(id);
            if (c == null) return null;
            return new Cliente
            {
                Id = c.Id,
                Nome = c.Nome,
                Email = c.Email,
                Telefone = c.Telefone,
            };
        }

        public async Task CriarAsync(UsuarioViewModel usuarioVm, ClienteViewModel clienteVm)
        {
            var senha = EncryptUtils.EncryptPasswordBase64(usuarioVm.Senha);
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                NomeUsuario = usuarioVm.NomeUsuario,
                Senha = EncryptUtils.EncryptPassword(senha)
            };
            await _usuarioRepo.AddAsync(usuario);

            var cliente = new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = clienteVm.Nome,
                Email = clienteVm.Email,
                Telefone = clienteVm.Telefone,
                UsuarioCliente = usuario
            };
            await _repo.AddAsync(cliente);
        }

        public async Task AtualizarAsync(ClienteViewModel vm)
        {
            var entity = new Cliente
            {
                Nome = vm.Nome,
                Email = vm.Email,
                Telefone = vm.Telefone
            };
            await _repo.UpdateAsync(entity);
        }

        public async Task ExcluirAsync(Guid? id) =>
            await _repo.DeleteAsync(id);
    }
}
