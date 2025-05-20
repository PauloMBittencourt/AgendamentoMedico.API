using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendamentoMedico.Infra.Migrations
{
    /// <inheritdoc />
    public partial class CorreçãodoCargosIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Perfis_Funcionarios_CargosIdentity_CargosIdentityId",
                table: "Perfis_Funcionarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Perfis_Funcionarios_Usuarios_UsuarioId",
                table: "Perfis_Funcionarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Perfis_Funcionarios",
                table: "Perfis_Funcionarios");

            migrationBuilder.RenameTable(
                name: "Perfis_Funcionarios",
                newName: "CargosIdentity_Usuarios");

            migrationBuilder.RenameIndex(
                name: "IX_Perfis_Funcionarios_UsuarioId",
                table: "CargosIdentity_Usuarios",
                newName: "IX_CargosIdentity_Usuarios_UsuarioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CargosIdentity_Usuarios",
                table: "CargosIdentity_Usuarios",
                columns: new[] { "CargosIdentityId", "UsuarioId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CargosIdentity_Usuarios_CargosIdentity_CargosIdentityId",
                table: "CargosIdentity_Usuarios",
                column: "CargosIdentityId",
                principalTable: "CargosIdentity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CargosIdentity_Usuarios_Usuarios_UsuarioId",
                table: "CargosIdentity_Usuarios",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CargosIdentity_Usuarios_CargosIdentity_CargosIdentityId",
                table: "CargosIdentity_Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_CargosIdentity_Usuarios_Usuarios_UsuarioId",
                table: "CargosIdentity_Usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CargosIdentity_Usuarios",
                table: "CargosIdentity_Usuarios");

            migrationBuilder.RenameTable(
                name: "CargosIdentity_Usuarios",
                newName: "Perfis_Funcionarios");

            migrationBuilder.RenameIndex(
                name: "IX_CargosIdentity_Usuarios_UsuarioId",
                table: "Perfis_Funcionarios",
                newName: "IX_Perfis_Funcionarios_UsuarioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Perfis_Funcionarios",
                table: "Perfis_Funcionarios",
                columns: new[] { "CargosIdentityId", "UsuarioId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Perfis_Funcionarios_CargosIdentity_CargosIdentityId",
                table: "Perfis_Funcionarios",
                column: "CargosIdentityId",
                principalTable: "CargosIdentity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Perfis_Funcionarios_Usuarios_UsuarioId",
                table: "Perfis_Funcionarios",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }
    }
}
