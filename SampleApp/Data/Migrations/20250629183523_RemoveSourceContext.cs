using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandboxCmd.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSourceContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SourceContext",
                schema: "serilog");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SourceContext",
                schema: "serilog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    SourceContext = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceContext", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SourceContext_SourceContext_UserName",
                schema: "serilog",
                table: "SourceContext",
                columns: new[] { "SourceContext", "UserName" },
                unique: true);
        }
    }
}
