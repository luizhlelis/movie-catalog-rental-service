using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentMovie.Infrastructure.Migrations
{
    public partial class ModelUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: new Guid("4e3bb825-e44c-441f-bf65-75934f8dda5b"));

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "AmountAvailable", "CategoryName", "DirectorName", "ReleaseYear", "RentCategoryName", "Synopsis", "Title" },
                values: new object[] { new Guid("117fceed-173c-460f-b746-a619bf6862c5"), 10, null, null, 2022, null, "When the Riddler, a sadistic serial killer, begins murdering key political figures in Gotham, Batman is forced to investigate the city's hidden corruption and question his family's involvement.", "The Batman" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Username",
                keyValue: "admin-user",
                column: "PasswordHash",
                value: "$2a$11$a2y7pmVo8jTLq0u6buVRqOUD5s65mFyW7kdzMl16eADPPe6pHxnra");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Username",
                keyValue: "customer-user",
                column: "PasswordHash",
                value: "$2a$11$ycCqkfMk584VTmkWMybvrOTT16Dva46YCbj8OtRozjBaferedfUMG");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: new Guid("117fceed-173c-460f-b746-a619bf6862c5"));

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "AmountAvailable", "CategoryName", "DirectorName", "ReleaseYear", "RentCategoryName", "Synopsis", "Title" },
                values: new object[] { new Guid("4e3bb825-e44c-441f-bf65-75934f8dda5b"), 10, null, null, 2022, null, "When the Riddler, a sadistic serial killer, begins murdering key political figures in Gotham, Batman is forced to investigate the city's hidden corruption and question his family's involvement.", "The Batman" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Username",
                keyValue: "admin-user",
                column: "PasswordHash",
                value: "$2a$11$jcTH2SqP6l2Zg4ycYymSy.BzVlKWtIMhMi.sCgX6w5IIHIZwG9VfO");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Username",
                keyValue: "customer-user",
                column: "PasswordHash",
                value: "$2a$11$Dml4O83tKQS38fvjInWKsu6EggQqchHZ.Ef/3lNDkXvl6zKsXI3YS");
        }
    }
}
