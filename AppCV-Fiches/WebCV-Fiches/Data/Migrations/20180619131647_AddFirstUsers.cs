using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebCV_Fiches.Data.Migrations
{
    public partial class AddFirstUsers : Migration
    {
        string AdminUserId = Guid.NewGuid().ToString(), AdminRoleId = Guid.NewGuid().ToString();

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            PasswordHasher<Models.Admin.ApplicationUser> hasher = new PasswordHasher<Models.Admin.ApplicationUser>();

            migrationBuilder.InsertData("AspNetUsers", new string[]
            {
                "Id",
                "AccessFailedCount",
                "ConcurrencyStamp",
                "Email",
                "EmailConfirmed",
                "LockoutEnabled",
                "LockoutEnd",
                "NormalizedEmail",
                "NormalizedUserName",
                "PasswordHash",
                "PhoneNumber",
                "PhoneNumberConfirmed",
                "SecurityStamp",
                "TwoFactorEnabled",
                "UserName"
            },
            new object[]
            {
                AdminUserId, //Id
                0, //AccesFailes
                Guid.NewGuid().ToString(), //Concurrency
                "admin@lgs.com", //Email
                1, //ConfirmedEmail
                1, //Lockout enable
                null, //Lockout end:
                "ADMIN@LGS.COM",
                "ADMIN@LGS.COM",
                 hasher.HashPassword(new Models.Admin.ApplicationUser(), "Bonjour01#"),
                 "",
                 "",
                 Guid.NewGuid().ToString(),
                 0,
                 "admin@lgs.com"
            },
            "dbo");

            migrationBuilder.InsertData("AspNetRoles", new string[]
            {
                "Id",
                "ConcurrencyStamp",
                "Name",
                "NormalizedName"
            },
            new object[]
            {
                AdminRoleId,
                Guid.NewGuid().ToString(),
                "Administrateur",
                "ADMINISTRATEUR"
            },
            "dbo");

            migrationBuilder.InsertData("AspNetUserRoles", new string[]
            {
                "UserId",
                "RoleId"
            },
            new object[]
            {
                AdminUserId,
                AdminRoleId
            },
            "dbo");

            migrationBuilder.InsertData("AspNetRoles", new string[]
            {
                "Id",
                "ConcurrencyStamp",
                "Name",
                "NormalizedName"
            },
            new object[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                "Conseiller",
                "CONSEILLER"
            },
            "dbo");

            migrationBuilder.InsertData("AspNetRoles", new string[]
            {
                "Id",
                "ConcurrencyStamp",
                "Name",
                "NormalizedName"
            },
            new object[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                "Approbateur",
                "APPROBATEUR"
            },
            "dbo");

            var utilisateurGraphRepository = new UtilisateurGraphRepository();
            var utilisateur = new Utilisateur()
            {
                Prenom = "Admin",
                AdresseCourriel = "admin@lgs.com"
            };
            utilisateurGraphRepository.CreateIfNotExists(utilisateur);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM dbo.AspNetUserRoles");
            migrationBuilder.Sql("DELETE FROM dbo.AspNetRoles");
            migrationBuilder.Sql("DELETE FROM dbo.AspNetUsers");
        }
    }
}
