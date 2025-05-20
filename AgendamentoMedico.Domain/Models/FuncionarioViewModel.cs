using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Domain.Enuns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Domain.Models
{
    public class FuncionarioViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string NomeUsuario { get; set; }
        public Guid UsuarioId { get; set; }
        public Usuario UsuarioFuncionario { get; set; }
    }
}
