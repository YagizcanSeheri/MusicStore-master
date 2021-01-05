using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicStore.InfrastructureLayer.Migrations
{
    public partial class SpCallAddedToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROC uspp_GetCoverTypes 
                                    AS 
                                    BEGIN 
                                     SELECT * FROM   dbo.CoverTypes 
                                    END");

            migrationBuilder.Sql(@"CREATE PROC uspp_GetCoverType 
                                    @Id int 
                                    AS 
                                    BEGIN 
                                     SELECT * FROM   dbo.CoverTypes  WHERE  (Id = @Id) 
                                    END ");

            migrationBuilder.Sql(@"CREATE PROC uspp_UpdateCoverType
	                                @Id int,
	                                @Name varchar(100)
                                    AS 
                                    BEGIN 
                                     UPDATE dbo.CoverTypes
                                     SET  Name = @Name
                                     WHERE  Id = @Id
                                    END");

            migrationBuilder.Sql(@"CREATE PROC uspp_DeleteCoverType
	                                @Id int
                                    AS 
                                    BEGIN 
                                     DELETE FROM dbo.CoverTypes
                                     WHERE  Id = @Id
                                    END");

            migrationBuilder.Sql(@"CREATE PROC uspp_CreateCoverType
                                   @Name varchar(100)
                                   AS 
                                   BEGIN 
                                    INSERT INTO dbo.CoverTypes(Name)
                                    VALUES (@Name)
                                   END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE uspp_GetCoverTypes");
            migrationBuilder.Sql(@"DROP PROCEDURE uspp_GetCoverType");
            migrationBuilder.Sql(@"DROP PROCEDURE uspp_UpdateCoverType");
            migrationBuilder.Sql(@"DROP PROCEDURE uspp_DeleteCoverType");
            migrationBuilder.Sql(@"DROP PROCEDURE uspp_CreateCoverType");
        }
    }
}
