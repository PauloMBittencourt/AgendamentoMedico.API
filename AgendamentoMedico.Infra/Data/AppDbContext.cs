using Microsoft.EntityFrameworkCore;
using AgendamentoMedico.Domain.Entities;

namespace AgendamentoMedico.Infra.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Funcionario> Funcionarios { get; set; }   
        public DbSet<Cliente> Clientes { get; set; }     
        public DbSet<Usuario> Usuarios { get; set; } 
        public DbSet<IdentityRole> CargosIdentity { get; set; } 
        public DbSet<HorarioDisponivel> HorariosDisponiveis { get; set; }
        public DbSet<Funcionario_Cliente> Agendamentos { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Funcionario_Cliente>(fc =>
            {
                fc.HasKey(x => new { x.HorarioDisponivelId, x.ClienteId });

                fc.HasOne(x => x.HorarioDisponivel)
                  .WithMany(h => h.Agendamentos)
                  .HasForeignKey(x => x.HorarioDisponivelId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .IsRequired();

                fc.HasOne(x => x.ClienteFk)
                  .WithMany(c => c.Agendamentos)
                  .HasForeignKey(x => x.ClienteId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .IsRequired();
            });

            modelBuilder.Entity<HorarioDisponivel>(hd =>
            {
                hd.HasKey(x => x.HorarioDisponivelId);

                hd.HasOne(x => x.Funcionario)
                  .WithMany(f => f.HorariosDisponiveis)
                  .HasForeignKey(x => x.FuncionarioId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .IsRequired();
            });

            modelBuilder.Entity<Cliente>(c =>
            {
                c.HasKey(x => x.Id);

                c.Property(x => x.Nome)
                 .HasMaxLength(200)
                 .IsRequired();

                c.Property(x => x.Email)
                 .HasMaxLength(200)
                 .IsRequired();

                c.Property(x => x.Telefone)
                 .HasMaxLength(20);

                c.HasOne(x => x.UsuarioClienteId)
                 .WithOne(u => u.ClienteId)
                 .HasForeignKey<Cliente>(x => x.Id)       
                 .OnDelete(DeleteBehavior.Restrict);      

                c.HasMany(x => x.Agendamentos)
                 .WithOne(a => a.ClienteFk)
                 .HasForeignKey(a => a.ClienteId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Funcionario>(f =>
            {
                f.HasKey(x => x.Id);

                f.Property(x => x.Nome)
                 .HasMaxLength(200)
                 .IsRequired();

                f.Property(x => x.Email)
                 .HasMaxLength(200)
                 .IsRequired();

                f.Property(x => x.Cargo)
                 .IsRequired();

                f.HasOne(x => x.UsuarioFuncionarioId)
                 .WithOne(u => u.FuncionarioId)
                 .HasForeignKey<Funcionario>(x => x.Id)
                 .OnDelete(DeleteBehavior.Restrict);

                f.HasMany(x => x.HorariosDisponiveis)
                 .WithOne(h => h.Funcionario)
                 .HasForeignKey(h => h.FuncionarioId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Usuario>(u =>
            {
                u.HasKey(x => x.Id);

                u.Property(x => x.NomeUsuario)
                 .HasMaxLength(100)
                 .IsRequired();

                u.Property(x => x.Senha)
                 .HasMaxLength(200)
                 .IsRequired();
            });

            modelBuilder.Entity<IdentityRole>(r =>
            {
                r.HasKey(x => x.Id);

                r.Property(x => x.Nome)
                 .HasMaxLength(100)
                 .IsRequired();

                r.Property(x => x.Descricao)
                 .HasMaxLength(500);
            });
        }
    }
}