using FluentMigrator;

namespace CarRentalSystem.Migrations
{
    [Migration(1)] 
    public class CreateUsersTable : Migration
    {
        public override void Up()
        {
            Create.Table("Users")
                .WithColumn("UserID").AsInt32().PrimaryKey().Identity()
                .WithColumn("Username").AsString(50).NotNullable().Unique()
                .WithColumn("PasswordHash").AsString(255).NotNullable() // Stores the BCrypt hash, NEVER plain text
                .WithColumn("Role").AsString(20).NotNullable()          // Admin, Manager, Agent
                .WithColumn("FirstName").AsString(50).NotNullable()
                .WithColumn("LastName").AsString(50).NotNullable()
                .WithColumn("IsActive").AsBoolean().WithDefaultValue(true)
                .WithColumn("CreatedAt").AsDateTime().WithDefaultValue(SystemMethods.CurrentDateTime);
        }

        public override void Down()
        {
            Delete.Table("Users");
        }
    }
}