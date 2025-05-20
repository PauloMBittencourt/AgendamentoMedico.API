using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendamentoMedico.Infra.Migrations
{
    /// <inheritdoc />
    public partial class CorreçãodoGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Perfis_Funcionarios",
                columns: table => new
                {
                    CargosIdentityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfis_Funcionarios", x => new { x.CargosIdentityId, x.UsuarioId });
                    table.ForeignKey(
                        name: "FK_Perfis_Funcionarios_CargosIdentity_CargosIdentityId",
                        column: x => x.CargosIdentityId,
                        principalTable: "CargosIdentity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Perfis_Funcionarios_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Perfis_Funcionarios_UsuarioId",
                table: "Perfis_Funcionarios",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Perfis_Funcionarios");
        }
    }
}
