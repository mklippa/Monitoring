using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MonitoringService.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgentInfos",
                columns: table => new
                {
                    AgentInfoId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AgentId = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    ReportId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentInfos", x => x.AgentInfoId);
                    table.ForeignKey(
                        name: "FK_AgentInfos_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Errors",
                columns: table => new
                {
                    ErrorId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AgentInfoId = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Errors", x => x.ErrorId);
                    table.ForeignKey(
                        name: "FK_Errors_AgentInfos_AgentInfoId",
                        column: x => x.AgentInfoId,
                        principalTable: "AgentInfos",
                        principalColumn: "AgentInfoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgentInfos_ReportId",
                table: "AgentInfos",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Errors_AgentInfoId",
                table: "Errors",
                column: "AgentInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Errors");

            migrationBuilder.DropTable(
                name: "AgentInfos");

            migrationBuilder.DropTable(
                name: "Reports");
        }
    }
}
