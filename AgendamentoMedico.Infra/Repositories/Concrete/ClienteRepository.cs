using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Infra.Data;
using AgendamentoMedico.Infra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Infra.Repositories.Concrete
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;
        public ClienteRepository(AppDbContext ctx) => _context = ctx;

        public async Task<List<Cliente>> GetAllAsync() =>
            await _context.Clientes.Include(c => c.UsuarioCliente).ToListAsync();

        public async Task<Cliente?> GetByIdAsync(Guid? id) =>
            await _context.Clientes
                      .Include(c => c.UsuarioCliente)
                      .FirstOrDefaultAsync(c => c.Id == id);

        public async Task AddAsync(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid? id)
        {
            var c = new Cliente { Id = (Guid)id };
            _context.Clientes.Remove(c);
            await _context.SaveChangesAsync();
        }
    }
}
