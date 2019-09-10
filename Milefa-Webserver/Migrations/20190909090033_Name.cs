using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Milefa_Webserver.Migrations
{
    public partial class Name : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    MaxEmployes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    School = table.Column<string>(maxLength: 50, nullable: true),
                    _Class = table.Column<string>(maxLength: 50, nullable: true),
                    Gender = table.Column<int>(nullable: true),
                    DateValide = table.Column<DateTime>(nullable: false),
                    PersNr = table.Column<int>(nullable: false),
                    Breakfast = table.Column<bool>(nullable: false),
                    Lunch = table.Column<bool>(nullable: false),
                    DeployedDepID = table.Column<int>(nullable: true),
                    Choise1ID = table.Column<int>(nullable: true),
                    Choise2ID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Student_Department_Choise1ID",
                        column: x => x.Choise1ID,
                        principalTable: "Department",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Student_Department_Choise2ID",
                        column: x => x.Choise2ID,
                        principalTable: "Department",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Student_Department_DeployedDepID",
                        column: x => x.DeployedDepID,
                        principalTable: "Department",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Skill",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    DepartmentID = table.Column<int>(nullable: true),
                    StudentID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Skill_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Department",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skill_Student_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Student",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Skill_DepartmentID",
                table: "Skill",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_StudentID",
                table: "Skill",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Choise1ID",
                table: "Student",
                column: "Choise1ID");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Choise2ID",
                table: "Student",
                column: "Choise2ID");

            migrationBuilder.CreateIndex(
                name: "IX_Student_DeployedDepID",
                table: "Student",
                column: "DeployedDepID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Skill");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropTable(
                name: "Department");
        }
    }
}
