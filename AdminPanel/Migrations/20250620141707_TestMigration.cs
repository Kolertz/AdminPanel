using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPanel.Migrations;

/// <inheritdoc />
public partial class TestMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Tags_Clients_ClientId",
            table: "Tags");

        migrationBuilder.DropIndex(
            name: "IX_Tags_ClientId",
            table: "Tags");

        migrationBuilder.DropColumn(
            name: "ClientId",
            table: "Tags");

        migrationBuilder.CreateTable(
            name: "ClientTag",
            columns: table => new
            {
                ClientsId = table.Column<int>(type: "integer", nullable: false),
                TagsId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ClientTag", x => new { x.ClientsId, x.TagsId });
                table.ForeignKey(
                    name: "FK_ClientTag_Clients_ClientsId",
                    column: x => x.ClientsId,
                    principalTable: "Clients",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ClientTag_Tags_TagsId",
                    column: x => x.TagsId,
                    principalTable: "Tags",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ClientTag_TagsId",
            table: "ClientTag",
            column: "TagsId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ClientTag");

        migrationBuilder.AddColumn<int>(
            name: "ClientId",
            table: "Tags",
            type: "integer",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Tags_ClientId",
            table: "Tags",
            column: "ClientId");

        migrationBuilder.AddForeignKey(
            name: "FK_Tags_Clients_ClientId",
            table: "Tags",
            column: "ClientId",
            principalTable: "Clients",
            principalColumn: "Id");
    }
}
