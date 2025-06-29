using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandboxCmd.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SourceContexts",
                table: "SourceContexts");

            migrationBuilder.DropIndex(
                name: "IX_SourceContexts_SourceContext_Level_UserName",
                table: "SourceContexts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SerilogTableMarkers",
                table: "SerilogTableMarkers");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "SourceContexts");

            migrationBuilder.EnsureSchema(
                name: "serilog");

            migrationBuilder.RenameTable(
                name: "SourceContexts",
                newName: "SourceContext",
                newSchema: "serilog");

            migrationBuilder.RenameTable(
                name: "SerilogTableMarkers",
                newName: "SerilogTableMarker",
                newSchema: "serilog");

            migrationBuilder.RenameIndex(
                name: "IX_SerilogTableMarkers_SchemaName_TableName",
                schema: "serilog",
                table: "SerilogTableMarker",
                newName: "IX_SerilogTableMarker_SchemaName_TableName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SourceContext",
                schema: "serilog",
                table: "SourceContext",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SerilogTableMarker",
                schema: "serilog",
                table: "SerilogTableMarker",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SourceContext_SourceContext_UserName",
                schema: "serilog",
                table: "SourceContext",
                columns: new[] { "SourceContext", "UserName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SourceContext",
                schema: "serilog",
                table: "SourceContext");

            migrationBuilder.DropIndex(
                name: "IX_SourceContext_SourceContext_UserName",
                schema: "serilog",
                table: "SourceContext");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SerilogTableMarker",
                schema: "serilog",
                table: "SerilogTableMarker");

            migrationBuilder.RenameTable(
                name: "SourceContext",
                schema: "serilog",
                newName: "SourceContexts");

            migrationBuilder.RenameTable(
                name: "SerilogTableMarker",
                schema: "serilog",
                newName: "SerilogTableMarkers");

            migrationBuilder.RenameIndex(
                name: "IX_SerilogTableMarker_SchemaName_TableName",
                table: "SerilogTableMarkers",
                newName: "IX_SerilogTableMarkers_SchemaName_TableName");

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "SourceContexts",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SourceContexts",
                table: "SourceContexts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SerilogTableMarkers",
                table: "SerilogTableMarkers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SourceContexts_SourceContext_Level_UserName",
                table: "SourceContexts",
                columns: new[] { "SourceContext", "Level", "UserName" },
                unique: true);
        }
    }
}
