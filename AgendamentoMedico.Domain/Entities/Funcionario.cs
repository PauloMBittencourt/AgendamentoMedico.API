using AgendamentoMedico.Domain.Enuns;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendamentoMedico.Domain.Entities
{
    [Table("Funcionarios")]
    public class Funcionario
    {
        [Key]
        public Guid Id { get; set; }
        [Column("Nome")]
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        public string Nome { get; set; }
        [Column("Email")]
        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        public string Email { get; set; }
        [Column("Cargo")]
        [Required(ErrorMessage = "O campo Cargo é obrigatório.")]
        public EnumCargo Cargo { get; set; }

        public Usuario UsuarioFuncionarioId { get; set; }
        public ICollection<HorarioDisponivel> HorariosDisponiveis { get; set; }

    }
}
