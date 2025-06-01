using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public string Telefone { get; set; }


        [ForeignKey(nameof(UsuarioCliente))]
        public Guid UsuarioId { get; set; }
        [DisplayName("Login do Usuário")]
        public Usuario UsuarioCliente { get; set; }
        public ICollection<Funcionario_Cliente> Agendamentos { get; set; }
    }
}
