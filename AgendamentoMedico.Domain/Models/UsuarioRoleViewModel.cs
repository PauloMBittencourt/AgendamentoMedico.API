using AgendamentoMedico.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Domain.Models
{
    public class UsuarioRoleViewModel
    {
        public Guid UsuarioId { get; set; }
        public string Nome { get; set; }
        public List<string>? Roles { get; set; }
        public List<IdentityRole>? RolesLista { get; set; }
    }
}
