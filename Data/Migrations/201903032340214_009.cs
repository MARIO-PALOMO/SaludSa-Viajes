namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _009 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RolAdministrativo", "CreadoPor", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.RolAdministrativo", "FechaCreacion", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RolAdministrativo", "FechaCreacion");
            DropColumn("dbo.RolAdministrativo", "CreadoPor");
        }
    }
}
