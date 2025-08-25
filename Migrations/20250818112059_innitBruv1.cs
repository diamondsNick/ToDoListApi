using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoListApi.Migrations
{
    /// <inheritdoc />
    public partial class innitBruv1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ToDos",
                newName: "Assignments");

            migrationBuilder.RenameTable(
                name: "Status",
                newName: "Statuses");

            migrationBuilder.RenameTable(
                name: "AssigmentPages",
                newName: "AssignmentsPages");

            migrationBuilder.RenameIndex(
                name: "IX_ToDos_StatusId",
                table: "Assignments",
                newName: "IX_Assignments_StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_AssigmentPages_PageId",
                table: "AssignmentsPages",
                newName: "IX_AssignmentsPages_PageId");

            migrationBuilder.RenameIndex(
                name: "IX_AssigmentPages_AssignmentId",
                table: "AssignmentsPages",
                newName: "IX_AssignmentsPages_AssignmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Statuses",
                newName: "Status");

            migrationBuilder.RenameTable(
                name: "AssignmentsPages",
                newName: "AssigmentPages");

            migrationBuilder.RenameTable(
                name: "Assignments",
                newName: "ToDos");

            migrationBuilder.RenameIndex(
                name: "IX_AssignmentsPages_PageId",
                table: "AssigmentPages",
                newName: "IX_AssigmentPages_PageId");

            migrationBuilder.RenameIndex(
                name: "IX_AssignmentsPages_AssignmentId",
                table: "AssigmentPages",
                newName: "IX_AssigmentPages_AssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Assignments_StatusId",
                table: "ToDos",
                newName: "IX_ToDos_StatusId");
        }
    }
}
