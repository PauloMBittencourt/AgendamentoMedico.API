using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Domain.Entities
{
    [Table("Clientes")]
    public class Cliente
    {
        [Key]
        public Guid Id { get; set; }
        [Column("Nome")]
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        public string Email { get; set; }
        public string Telefone { get; set; }

        public Usuario UsuarioClienteId { get; set; }
        public ICollection<Funcionario_Cliente> Agendamentos { get; set; }
    }
}
