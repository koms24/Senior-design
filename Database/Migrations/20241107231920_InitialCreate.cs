using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    Uid = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: true),
                    StateId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.Uid);
                });

            migrationBuilder.CreateTable(
                name: "StateType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParentItemId = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<string>(type: "text", nullable: false),
                    DisplayState = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    NumericState = table.Column<decimal>(type: "numeric", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StateType_Item_ParentItemId",
                        column: x => x.ParentItemId,
                        principalTable: "Item",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Item_StateId",
                table: "Item",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_StateType_ParentItemId",
                table: "StateType",
                column: "ParentItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_StateType_StateId",
                table: "Item",
                column: "StateId",
                principalTable: "StateType",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_StateType_StateId",
                table: "Item");

            migrationBuilder.DropTable(
                name: "StateType");

            migrationBuilder.DropTable(
                name: "Item");
        }
    }
}
