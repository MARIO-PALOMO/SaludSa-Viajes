namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _013 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.RolAdministrativo", "ColaboradorCiudadCodigo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RolAdministrativo", "ColaboradorCiudadCodigo", c => c.String(nullable: false, maxLength: 100));
        }
    }
}
