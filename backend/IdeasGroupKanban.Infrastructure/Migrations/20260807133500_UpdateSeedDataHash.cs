using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeasGroupKanban.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedDataHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$i/pzsIcwIc6lwhEApYEgcu/iqq7ikESZhsLWI1d1Kn2dUBlKh7sBy");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "PasswordHash",
                value: "$2a$11$i/pzsIcwIc6lwhEApYEgcu/iqq7ikESZhsLWI1d1Kn2dUBlKh7sBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$s1s4c.H/rQG2v1T0.mE9oOWo/FvR1F3Y6bN1t9K8c1O0L1y.e8qC.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "PasswordHash",
                value: "$2a$11$s1s4c.H/rQG2v1T0.mE9oOWo/FvR1F3Y6bN1t9K8c1O0L1y.e8qC.");
        }
    }
}
