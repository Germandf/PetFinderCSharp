using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PetFinder.Migrations
{
    public partial class InitialSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "AnimalTypes",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>("nvarchar(max)", nullable: true),
                    SerializedName = table.Column<string>("nvarchar(max)", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_AnimalTypes", x => x.Id); });

            migrationBuilder.CreateTable(
                "AspNetRoles",
                table => new
                {
                    Id = table.Column<string>("nvarchar(450)", nullable: false),
                    Name = table.Column<string>("nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>("nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>("nvarchar(max)", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_AspNetRoles", x => x.Id); });

            migrationBuilder.CreateTable(
                "AspNetUsers",
                table => new
                {
                    Id = table.Column<string>("nvarchar(450)", nullable: false),
                    Name = table.Column<string>("nvarchar(max)", nullable: true),
                    Surname = table.Column<string>("nvarchar(max)", nullable: true),
                    UserName = table.Column<string>("nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>("nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>("nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>("nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>("bit", nullable: false),
                    PasswordHash = table.Column<string>("nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>("nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>("nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>("nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>("bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>("bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>("datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>("bit", nullable: false),
                    AccessFailedCount = table.Column<int>("int", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_AspNetUsers", x => x.Id); });

            migrationBuilder.CreateTable(
                "Cities",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>("nvarchar(max)", nullable: true),
                    SerializedName = table.Column<string>("nvarchar(max)", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Cities", x => x.Id); });

            migrationBuilder.CreateTable(
                "Genders",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>("nvarchar(max)", nullable: true),
                    SerializedName = table.Column<string>("nvarchar(max)", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Genders", x => x.Id); });

            migrationBuilder.CreateTable(
                "AspNetRoleClaims",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>("nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>("nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>("nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        x => x.RoleId,
                        "AspNetRoles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserClaims",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>("nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>("nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>("nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        "FK_AspNetUserClaims_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserLogins",
                table => new
                {
                    LoginProvider = table.Column<string>("nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>("nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>("nvarchar(max)", nullable: true),
                    UserId = table.Column<string>("nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new {x.LoginProvider, x.ProviderKey});
                    table.ForeignKey(
                        "FK_AspNetUserLogins_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserRoles",
                table => new
                {
                    UserId = table.Column<string>("nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>("nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new {x.UserId, x.RoleId});
                    table.ForeignKey(
                        "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        x => x.RoleId,
                        "AspNetRoles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_AspNetUserRoles_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserTokens",
                table => new
                {
                    UserId = table.Column<string>("nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>("nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>("nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>("nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new {x.UserId, x.LoginProvider, x.Name});
                    table.ForeignKey(
                        "FK_AspNetUserTokens_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Comments",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>("nvarchar(max)", nullable: true),
                    UserId = table.Column<string>("nvarchar(450)", nullable: true),
                    PetId = table.Column<int>("int", nullable: false),
                    Rate = table.Column<int>("int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        "FK_Comments_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Pets",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>("nvarchar(max)", nullable: true),
                    AnimalTypeId = table.Column<int>("int", nullable: false),
                    CityId = table.Column<int>("int", nullable: false),
                    GenderId = table.Column<int>("int", nullable: false),
                    Date = table.Column<DateTime>("datetime2", nullable: false),
                    PhoneNumber = table.Column<string>("nvarchar(max)", nullable: true),
                    Photo = table.Column<string>("nvarchar(max)", nullable: true),
                    Description = table.Column<string>("nvarchar(max)", nullable: true),
                    UserId = table.Column<string>("nvarchar(450)", nullable: true),
                    Found = table.Column<byte>("tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pets", x => x.Id);
                    table.ForeignKey(
                        "FK_Pets_AnimalTypes_AnimalTypeId",
                        x => x.AnimalTypeId,
                        "AnimalTypes",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_Pets_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Pets_Cities_CityId",
                        x => x.CityId,
                        "Cities",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_Pets_Genders_GenderId",
                        x => x.GenderId,
                        "Genders",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(
                "CREATE FULLTEXT CATALOG PetFinder_Catalog AS DEFAULT;",
                true);

            migrationBuilder.Sql(
                "CREATE FULLTEXT INDEX ON [dbo].[Pets](" +
                "[Description] LANGUAGE 'Spanish', " +
                "[Name] LANGUAGE 'Spanish')" +
                "KEY INDEX[PK_Pets]ON([PetFinder_Catalog], FILEGROUP[PRIMARY])" +
                "WITH(CHANGE_TRACKING = AUTO, STOPLIST = SYSTEM)",
                true);

            migrationBuilder.CreateTable(
                "Logs",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>("nvarchar(MAX)", nullable: true),
                    MessageTemplate = table.Column<string>("nvarchar(MAX)", nullable: true),
                    Level = table.Column<string>("nvarchar(MAX)", nullable: false),
                    TimeStamp = table.Column<DateTime>("datetime", nullable: false),
                    Exception = table.Column<string>("nvarchar(MAX)", nullable: true),
                    Properties = table.Column<string>("nvarchar(MAX)", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Logs", x => x.Id); }
            );
            migrationBuilder.InsertData(
                "Genders",
                new[] {"Id", "Name", "SerializedName"},
                new object[] {1, "Macho", "MACHO"});

            migrationBuilder.InsertData(
                "Genders",
                new[] {"Id", "Name", "SerializedName"},
                new object[] {2, "Hembra", "HEMBRA"});

            migrationBuilder.CreateIndex(
                "IX_AspNetRoleClaims_RoleId",
                "AspNetRoleClaims",
                "RoleId");

            migrationBuilder.CreateIndex(
                "RoleNameIndex",
                "AspNetRoles",
                "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                "IX_AspNetUserClaims_UserId",
                "AspNetUserClaims",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_AspNetUserLogins_UserId",
                "AspNetUserLogins",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_AspNetUserRoles_RoleId",
                "AspNetUserRoles",
                "RoleId");

            migrationBuilder.CreateIndex(
                "EmailIndex",
                "AspNetUsers",
                "NormalizedEmail");

            migrationBuilder.CreateIndex(
                "UserNameIndex",
                "AspNetUsers",
                "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                "IX_Comments_UserId",
                "Comments",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_Pets_AnimalTypeId",
                "Pets",
                "AnimalTypeId");

            migrationBuilder.CreateIndex(
                "IX_Pets_CityId",
                "Pets",
                "CityId");

            migrationBuilder.CreateIndex(
                "IX_Pets_GenderId",
                "Pets",
                "GenderId");

            migrationBuilder.CreateIndex(
                "IX_Pets_UserId",
                "Pets",
                "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Logs");
            migrationBuilder.DropTable(
                "AspNetRoleClaims");

            migrationBuilder.DropTable(
                "AspNetUserClaims");

            migrationBuilder.DropTable(
                "AspNetUserLogins");

            migrationBuilder.DropTable(
                "AspNetUserRoles");

            migrationBuilder.DropTable(
                "AspNetUserTokens");

            migrationBuilder.DropTable(
                "Comments");

            migrationBuilder.DropTable(
                "Pets");

            migrationBuilder.DropTable(
                "AspNetRoles");

            migrationBuilder.DropTable(
                "AnimalTypes");

            migrationBuilder.DropTable(
                "AspNetUsers");

            migrationBuilder.DropTable(
                "Cities");

            migrationBuilder.DropTable(
                "Genders");
        }
    }
}