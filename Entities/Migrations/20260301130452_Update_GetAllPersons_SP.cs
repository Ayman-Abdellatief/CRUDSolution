using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class Update_GetAllPersons_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_getAllPersons = @"
        ALTER PROCEDURE [dbo].[GetAllPersons]
        AS
        BEGIN
            SELECT 
                PersonID,
                PersonName,
                Email,
                DateOfBirth,
                GenderId,   -- added column
                Gender,
                CountryID,
                Address,
                ReceiveNewsLetters
            FROM [dbo].[Persons]
        END";

            migrationBuilder.Sql(sp_getAllPersons);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_getAllPersons = @"
        ALTER PROCEDURE [dbo].[GetAllPersons]
        AS
        BEGIN
            SELECT 
                PersonID,
                PersonName,
                Email,
                DateOfBirth,
                Gender,
                CountryID,
                Address,
                ReceiveNewsLetters
            FROM [dbo].[Persons]
        END";

            migrationBuilder.Sql(sp_getAllPersons);
        }
    }
}
