using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentMovie.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actors",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actors", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Directors",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directors", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "MovieCategories",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieCategories", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "RentCategories",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentCategories", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    GivenName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Synopsis = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: false),
                    ReleaseYear = table.Column<int>(type: "int", nullable: false),
                    AmountAvailable = table.Column<int>(type: "int", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    RentCategoryName = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    DirectorName = table.Column<string>(type: "nvarchar(30)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movies_Directors_DirectorName",
                        column: x => x.DirectorName,
                        principalTable: "Directors",
                        principalColumn: "Name");
                    table.ForeignKey(
                        name: "FK_Movies_MovieCategories_CategoryName",
                        column: x => x.CategoryName,
                        principalTable: "MovieCategories",
                        principalColumn: "Name");
                    table.ForeignKey(
                        name: "FK_Movies_RentCategories_RentCategoryName",
                        column: x => x.RentCategoryName,
                        principalTable: "RentCategories",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalPrice = table.Column<double>(type: "float", nullable: false),
                    ShippingPrice = table.Column<double>(type: "float", nullable: false),
                    ItemsTotalPrice = table.Column<double>(type: "float", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CustomerUsername = table.Column<string>(type: "nvarchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_CustomerUsername",
                        column: x => x.CustomerUsername,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActorMovie",
                columns: table => new
                {
                    CastName = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    MoviesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActorMovie", x => new { x.CastName, x.MoviesId });
                    table.ForeignKey(
                        name: "FK_ActorMovie_Actors_CastName",
                        column: x => x.CastName,
                        principalTable: "Actors",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActorMovie_Movies_MoviesId",
                        column: x => x.MoviesId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Actors",
                column: "Name",
                values: new object[]
                {
                    "Colin Farrell",
                    "Paul Dano",
                    "Robert Pattinson",
                    "Zoë Kravitz"
                });

            migrationBuilder.InsertData(
                table: "Directors",
                column: "Name",
                value: "Matt Reeves");

            migrationBuilder.InsertData(
                table: "MovieCategories",
                column: "Name",
                value: "Action");

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "AmountAvailable", "CategoryName", "DirectorName", "ReleaseYear", "RentCategoryName", "Synopsis", "Title" },
                values: new object[] { new Guid("4e3bb825-e44c-441f-bf65-75934f8dda5b"), 10, null, null, 2022, null, "When the Riddler, a sadistic serial killer, begins murdering key political figures in Gotham, Batman is forced to investigate the city's hidden corruption and question his family's involvement.", "The Batman" });

            migrationBuilder.InsertData(
                table: "RentCategories",
                columns: new[] { "Name", "Price" },
                values: new object[] { "Release", 5.0 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Username", "Address", "GivenName", "PasswordHash", "Role", "ZipCode" },
                values: new object[,]
                {
                    { "admin-user", "5036 Tierra Locks Suite 158", "Admin User", "$2a$11$jcTH2SqP6l2Zg4ycYymSy.BzVlKWtIMhMi.sCgX6w5IIHIZwG9VfO", 0, "980395900" },
                    { "customer-user", "570 Hackett Bridge", "Admin User", "$2a$11$Dml4O83tKQS38fvjInWKsu6EggQqchHZ.Ef/3lNDkXvl6zKsXI3YS", 1, "948019535" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActorMovie_MoviesId",
                table: "ActorMovie",
                column: "MoviesId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_CategoryName",
                table: "Movies",
                column: "CategoryName");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_DirectorName",
                table: "Movies",
                column: "DirectorName");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Id_Title",
                table: "Movies",
                columns: new[] { "Id", "Title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movies_RentCategoryName",
                table: "Movies",
                column: "RentCategoryName");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerUsername",
                table: "Orders",
                column: "CustomerUsername");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActorMovie");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Actors");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Directors");

            migrationBuilder.DropTable(
                name: "MovieCategories");

            migrationBuilder.DropTable(
                name: "RentCategories");
        }
    }
}
