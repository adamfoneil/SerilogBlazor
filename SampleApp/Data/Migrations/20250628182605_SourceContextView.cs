using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandboxCmd.Migrations
{
    /// <inheritdoc />
    public partial class SourceContextView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SourceContexts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceContext = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Level = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceContexts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SourceContexts_SourceContext_Level",
                table: "SourceContexts",
                columns: new[] { "SourceContext", "Level" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SourceContexts");
        }
    }
}
