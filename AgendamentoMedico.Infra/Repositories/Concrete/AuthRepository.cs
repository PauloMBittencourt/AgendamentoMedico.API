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
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;
        public AuthRepository(AppDbContext context)
            => _context = context;

        public async Task<Usuario?> GetByUsernameAsync(string nomeUsuario)
        {
            return await _context.Usuarios
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(u => u.NomeUsuario == nomeUsuario);
        }
    }
}
