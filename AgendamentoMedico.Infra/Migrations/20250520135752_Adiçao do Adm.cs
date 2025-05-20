using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgendamentoMedico.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AdiçaodoAdm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CargosIdentity",
                columns: new[] { "Id", "Descricao", "Nome" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Usuário Administrador do Sistema", "Administrador" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Usuário Cliente", "Cliente" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "NomeUsuario", "Senha" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), "admin", "tnBB3UYTrvvwAsJ8r/TxIdw483HvvZsGi6cdcTspvucUMNW/sXkjfa5gLkTD54J7" });

            migrationBuilder.InsertData(
                table: "CargosIdentity_Usuarios",
                columns: new[] { "CargosIdentityId", "UsuarioId" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("22222222-2222-2222-2222-222222222222") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CargosIdentity",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "CargosIdentity_Usuarios",
                keyColumns: new[] { "CargosIdentityId", "UsuarioId" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "CargosIdentity",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));
        }
    }
}
