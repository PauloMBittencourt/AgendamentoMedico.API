using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Domain.Entities
{
    public class IdentityRole_Usuario
    {
        [Key]
        [Column(Order = 0)]
        public Guid CargosIdentityId { get; set; }
        [Key]
        [Column(Order = 1)]
        public Guid UsuarioId { get; set; }

        [ForeignKey(nameof(CargosIdentityId))]
        public IdentityRole CargosIdentityFk { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario UsuarioFk { get; set; }
    }
}
