using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckoutEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderID",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PersonID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Orders_Person_PersonID",
                        column: x => x.PersonID,
                        principalTable: "Person",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Snacks",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snacks", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SnackOrders",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    SnackID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PriceAtPurchase = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SnackOrders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SnackOrders_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SnackOrders_Snacks_SnackID",
                        column: x => x.SnackID,
                        principalTable: "Snacks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_OrderID",
                table: "Tickets",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PersonID",
                table: "Orders",
                column: "PersonID");

            migrationBuilder.CreateIndex(
                name: "IX_SnackOrders_OrderID",
                table: "SnackOrders",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_SnackOrders_SnackID",
                table: "SnackOrders",
                column: "SnackID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Orders_OrderID",
                table: "Tickets",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Orders_OrderID",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "SnackOrders");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Snacks");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_OrderID",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "OrderID",
                table: "Tickets");
        }
    }
}
