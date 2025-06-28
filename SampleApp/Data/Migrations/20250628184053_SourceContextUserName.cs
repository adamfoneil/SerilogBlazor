using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandboxCmd.Migrations
{
    /// <inheritdoc />
    public partial class SourceContextUserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SourceContexts_SourceContext_Level",
                table: "SourceContexts");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "SourceContexts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SourceContexts_SourceContext_Level_UserName",
                table: "SourceContexts",
                columns: new[] { "SourceContext", "Level", "UserName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SourceContexts_SourceContext_Level_UserName",
                table: "SourceContexts");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "SourceContexts");

            migrationBuilder.CreateIndex(
                name: "IX_SourceContexts_SourceContext_Level",
                table: "SourceContexts",
                columns: new[] { "SourceContext", "Level" },
                unique: true);
        }
    }
}
