using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ARID.Migrations
{
    public partial class edit1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
      name: "CourseChapterExams",
      columns: table => new
      {
          Id = table.Column<int>(nullable: false)
              .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
          CourseChapterId = table.Column<Guid>(nullable: false),
          Description = table.Column<string>(maxLength: 100, nullable: true),
          Imgurl = table.Column<string>(maxLength: 100, nullable: true),
          Indx = table.Column<int>(nullable: false),
          IsHidden = table.Column<bool>(nullable: false),
          Question = table.Column<string>(maxLength: 300, nullable: false)
      },
      constraints: table =>
      {
          table.PrimaryKey("PK_CourseChapterExams", x => x.Id);
          table.ForeignKey(
              name: "FK_CourseChapterExams_CourseChapters_CourseChapterId",
              column: x => x.CourseChapterId,
              principalTable: "CourseChapters",
              principalColumn: "Id",
              onDelete: ReferentialAction.Restrict);
      });

            migrationBuilder.CreateTable(
                name: "CourseChapterChoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CourseChapterExamId = table.Column<int>(nullable: false),
                    IsCorrectAnswer = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Mark = table.Column<int>(nullable: false),
                    Option = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseChapterChoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseChapterChoices_CourseChapterExams_CourseChapterExamId",
                        column: x => x.CourseChapterExamId,
                        principalTable: "CourseChapterExams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChapterExamLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnswerDate = table.Column<DateTime>(nullable: false),
                    ApplicationUserId = table.Column<string>(maxLength: 450, nullable: true),
                    CourseChapterChoiceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterExamLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChapterExamLogs_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChapterExamLogs_CourseChapterChoices_CourseChapterChoiceId",
                        column: x => x.CourseChapterChoiceId,
                        principalTable: "CourseChapterChoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });


            migrationBuilder.CreateIndex(
                name: "IX_CourseChapterExams_CourseChapterId",
                table: "CourseChapterExams",
                column: "CourseChapterId");
            migrationBuilder.CreateIndex(
                name: "IX_ChapterExamLogs_ApplicationUserId",
                table: "ChapterExamLogs",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChapterExamLogs_CourseChapterChoiceId",
                table: "ChapterExamLogs",
                column: "CourseChapterChoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseChapterChoices_CourseChapterExamId",
                table: "CourseChapterChoices",
                column: "CourseChapterExamId");

        }

    }
}
