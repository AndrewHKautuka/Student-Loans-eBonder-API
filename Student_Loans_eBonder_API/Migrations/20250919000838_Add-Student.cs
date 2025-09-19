using System;

using Microsoft.EntityFrameworkCore.Migrations;

using Student_Loans_eBonder_API.Student.Types.Models;

#nullable disable

namespace Student_Loans_eBonder_API.Migrations
{
	/// <inheritdoc />
	public partial class AddStudent : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterDatabase()
				.Annotation("Npgsql:Enum:name_component_type", "family,given,middle,particle")
				.Annotation("Npgsql:Enum:sex", "female,male")
				.OldAnnotation("Npgsql:Enum:name_component_type", "family,given,middle,particle");

			migrationBuilder.CreateTable(
				name: "students",
				columns: table => new
				{
					id = table.Column<Guid>(type: "uuid", nullable: false),
					user_id = table.Column<string>(type: "text", nullable: false),
					national_id_number = table.Column<string>(type: "text", nullable: true),
					national_id_scan_url = table.Column<string>(type: "text", nullable: true),
					date_of_birth = table.Column<DateOnly>(type: "date", nullable: true),
					sex = table.Column<Sex>(type: "sex", nullable: true),
					postal_address = table.Column<string>(type: "text", nullable: true),
					created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
					updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
				},
				constraints: table =>
				{
					table.PrimaryKey("pk_students", x => x.id);
					table.ForeignKey(
						name: "fk_students_users_user_id",
						column: x => x.user_id,
						principalTable: "user",
						principalColumn: "id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "ix_students_user_id",
				table: "students",
				column: "user_id",
				unique: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "students");

			migrationBuilder.AlterDatabase()
				.Annotation("Npgsql:Enum:name_component_type", "family,given,middle,particle")
				.OldAnnotation("Npgsql:Enum:name_component_type", "family,given,middle,particle")
				.OldAnnotation("Npgsql:Enum:sex", "female,male");
		}
	}
}
