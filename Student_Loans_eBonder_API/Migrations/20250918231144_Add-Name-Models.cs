using System;

using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

using Student_Loans_eBonder_API.Profile.Types.Models;

#nullable disable

namespace Student_Loans_eBonder_API.Migrations
{
	/// <inheritdoc />
	public partial class AddNameModels : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterDatabase()
				.Annotation("Npgsql:Enum:name_component_type", "family,given,middle,particle");

			migrationBuilder.CreateTable(
				name: "names",
				columns: table => new
				{
					id = table.Column<long>(type: "bigint", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					preferred_short_name = table.Column<string>(type: "text", nullable: true),
					created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
					updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
				},
				constraints: table =>
				{
					table.PrimaryKey("pk_names", x => x.id);
				});

			migrationBuilder.CreateTable(
				name: "name_components",
				columns: table => new
				{
					name_id = table.Column<long>(type: "bigint", nullable: false),
					position_order = table.Column<short>(type: "smallint", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					type = table.Column<NameComponentType>(type: "name_component_type", nullable: false),
					value = table.Column<string>(type: "text", nullable: false),
					created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
					updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
				},
				constraints: table =>
				{
					table.PrimaryKey("pk_name_components", x => new { x.name_id, x.position_order });
					table.ForeignKey(
						name: "fk_name_components_names_name_id",
						column: x => x.name_id,
						principalTable: "names",
						principalColumn: "id",
						onDelete: ReferentialAction.Cascade);
				});
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "name_components");

			migrationBuilder.DropTable(
				name: "names");

			migrationBuilder.AlterDatabase()
				.OldAnnotation("Npgsql:Enum:name_component_type", "family,given,middle,particle");
		}
	}
}
