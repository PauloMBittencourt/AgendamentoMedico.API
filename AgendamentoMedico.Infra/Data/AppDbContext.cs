using Microsoft.EntityFrameworkCore;
using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Utils.Encrypt;

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
        public DbSet<IdentityRole_Usuario> CargosIdentity_Usuarios { get; set; }
        public DbSet<HorarioDisponivel> HorariosDisponiveis { get; set; }
        public DbSet<Funcionario_Cliente> Agendamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), 
                                   Nome = "Administrador", Descricao = "Usuário Administrador do Sistema" },
                new IdentityRole { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Nome = "Cliente", Descricao = "Usuário Cliente" }
            );

            var senha = EncryptUtils.EncryptPassword(EncryptUtils.EncryptPasswordBase64("SenhaForte@123"));

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    NomeUsuario = "admin",
                    Senha = senha
                }
            );

            modelBuilder.Entity<IdentityRole_Usuario>().HasData(
                new IdentityRole_Usuario
                {
                    CargosIdentityId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    UsuarioId = Guid.Parse("22222222-2222-2222-2222-222222222222")
                }
            );

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

            modelBuilder.Entity<IdentityRole_Usuario>(pf =>
            {

                pf.ToTable("CargosIdentity_Usuarios");
                pf.HasKey(pf => new { pf.CargosIdentityId, pf.UsuarioId });

                pf.HasOne(pf => pf.CargosIdentityFk)
                        .WithMany(p => p.UsuarioFk)
                        .HasForeignKey(pf => pf.CargosIdentityId)
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                pf.HasOne(pf => pf.UsuarioFk)
                        .WithMany(f => f.Cargos)
                        .HasForeignKey(pf => pf.UsuarioId)
                        .OnDelete(DeleteBehavior.ClientCascade)
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

                c.HasOne(x => x.UsuarioCliente)
                 .WithOne(u => u.ClienteId)
                 .HasForeignKey<Cliente>(x => x.UsuarioId)
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

                f.HasOne(x => x.UsuarioFuncionario)
                 .WithOne(u => u.FuncionarioId)
                 .HasForeignKey<Funcionario>(x => x.UsuarioId)
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