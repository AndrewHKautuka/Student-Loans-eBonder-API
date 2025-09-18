using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Loans_eBonder_API.Migrations
{
	/// <inheritdoc />
	public partial class AddUserProfile : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "user_profiles",
				columns: table => new
				{
					user_id = table.Column<string>(type: "text", nullable: false),
					name_id = table.Column<long>(type: "bigint", nullable: false),
					profile_picture_url = table.Column<string>(type: "text", nullable: true),
					signature_scan_url = table.Column<string>(type: "text", nullable: true),
					created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
					updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
				},
				constraints: table =>
				{
					table.PrimaryKey("pk_user_profiles", x => x.user_id);
					table.ForeignKey(
						name: "fk_user_profiles_names_name_id",
						column: x => x.name_id,
						principalTable: "names",
						principalColumn: "id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "fk_user_profiles_users_user_id",
						column: x => x.user_id,
						principalTable: "user",
						principalColumn: "id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "ix_user_profiles_name_id",
				table: "user_profiles",
				column: "name_id");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "user_profiles");
		}
	}
}
