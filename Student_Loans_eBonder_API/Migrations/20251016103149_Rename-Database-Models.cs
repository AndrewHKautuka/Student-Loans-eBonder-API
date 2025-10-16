using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Loans_eBonder_API.Migrations
{
	/// <inheritdoc />
	public partial class RenameDatabaseModels : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "fk_name_components_names_name_id",
				table: "name_components");

			migrationBuilder.DropForeignKey(
				name: "fk_students_users_user_id",
				table: "students");

			migrationBuilder.DropForeignKey(
				name: "fk_user_profiles_names_name_id",
				table: "user_profiles");

			migrationBuilder.DropForeignKey(
				name: "fk_user_profiles_users_user_id",
				table: "user_profiles");

			migrationBuilder.DropPrimaryKey(
				name: "pk_user_profiles",
				table: "user_profiles");

			migrationBuilder.DropPrimaryKey(
				name: "pk_students",
				table: "students");

			migrationBuilder.DropPrimaryKey(
				name: "pk_names",
				table: "names");

			migrationBuilder.DropPrimaryKey(
				name: "pk_name_components",
				table: "name_components");

			migrationBuilder.RenameTable(
				name: "user_profiles",
				newName: "user_profile");

			migrationBuilder.RenameTable(
				name: "students",
				newName: "student");

			migrationBuilder.RenameTable(
				name: "names",
				newName: "name");

			migrationBuilder.RenameTable(
				name: "name_components",
				newName: "name_component");

			migrationBuilder.RenameIndex(
				name: "ix_user_profiles_name_id",
				table: "user_profile",
				newName: "ix_user_profile_name_id");

			migrationBuilder.RenameIndex(
				name: "ix_students_user_id",
				table: "student",
				newName: "ix_student_user_id");

			migrationBuilder.AddPrimaryKey(
				name: "pk_user_profile",
				table: "user_profile",
				column: "user_id");

			migrationBuilder.AddPrimaryKey(
				name: "pk_student",
				table: "student",
				column: "id");

			migrationBuilder.AddPrimaryKey(
				name: "pk_name",
				table: "name",
				column: "id");

			migrationBuilder.AddPrimaryKey(
				name: "pk_name_component",
				table: "name_component",
				columns: new[] { "name_id", "position_order" });

			migrationBuilder.AddForeignKey(
				name: "fk_name_component_name_name_id",
				table: "name_component",
				column: "name_id",
				principalTable: "name",
				principalColumn: "id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "fk_student_users_user_id",
				table: "student",
				column: "user_id",
				principalTable: "user",
				principalColumn: "id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "fk_user_profile_name_name_id",
				table: "user_profile",
				column: "name_id",
				principalTable: "name",
				principalColumn: "id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "fk_user_profile_users_user_id",
				table: "user_profile",
				column: "user_id",
				principalTable: "user",
				principalColumn: "id",
				onDelete: ReferentialAction.Cascade);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "fk_name_component_name_name_id",
				table: "name_component");

			migrationBuilder.DropForeignKey(
				name: "fk_student_users_user_id",
				table: "student");

			migrationBuilder.DropForeignKey(
				name: "fk_user_profile_name_name_id",
				table: "user_profile");

			migrationBuilder.DropForeignKey(
				name: "fk_user_profile_users_user_id",
				table: "user_profile");

			migrationBuilder.DropPrimaryKey(
				name: "pk_user_profile",
				table: "user_profile");

			migrationBuilder.DropPrimaryKey(
				name: "pk_student",
				table: "student");

			migrationBuilder.DropPrimaryKey(
				name: "pk_name_component",
				table: "name_component");

			migrationBuilder.DropPrimaryKey(
				name: "pk_name",
				table: "name");

			migrationBuilder.RenameTable(
				name: "user_profile",
				newName: "user_profiles");

			migrationBuilder.RenameTable(
				name: "student",
				newName: "students");

			migrationBuilder.RenameTable(
				name: "name_component",
				newName: "name_components");

			migrationBuilder.RenameTable(
				name: "name",
				newName: "names");

			migrationBuilder.RenameIndex(
				name: "ix_user_profile_name_id",
				table: "user_profiles",
				newName: "ix_user_profiles_name_id");

			migrationBuilder.RenameIndex(
				name: "ix_student_user_id",
				table: "students",
				newName: "ix_students_user_id");

			migrationBuilder.AddPrimaryKey(
				name: "pk_user_profiles",
				table: "user_profiles",
				column: "user_id");

			migrationBuilder.AddPrimaryKey(
				name: "pk_students",
				table: "students",
				column: "id");

			migrationBuilder.AddPrimaryKey(
				name: "pk_name_components",
				table: "name_components",
				columns: new[] { "name_id", "position_order" });

			migrationBuilder.AddPrimaryKey(
				name: "pk_names",
				table: "names",
				column: "id");

			migrationBuilder.AddForeignKey(
				name: "fk_name_components_names_name_id",
				table: "name_components",
				column: "name_id",
				principalTable: "names",
				principalColumn: "id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "fk_students_users_user_id",
				table: "students",
				column: "user_id",
				principalTable: "user",
				principalColumn: "id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "fk_user_profiles_names_name_id",
				table: "user_profiles",
				column: "name_id",
				principalTable: "names",
				principalColumn: "id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "fk_user_profiles_users_user_id",
				table: "user_profiles",
				column: "user_id",
				principalTable: "user",
				principalColumn: "id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}
