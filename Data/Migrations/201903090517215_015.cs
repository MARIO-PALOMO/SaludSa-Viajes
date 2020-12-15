namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _015 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RolAdministrativo", new[] { "CiudadId" });
            AlterColumn("dbo.RolAdministrativo", "EmpresaCodigo", c => c.String(maxLength: 100));
            AlterColumn("dbo.RolAdministrativo", "EmpresaNombre", c => c.String(maxLength: 300));
            AlterColumn("dbo.RolAdministrativo", "CiudadId", c => c.Long());
            CreateIndex("dbo.RolAdministrativo", "CiudadId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.RolAdministrativo", new[] { "CiudadId" });
            AlterColumn("dbo.RolAdministrativo", "CiudadId", c => c.Long(nullable: false));
            AlterColumn("dbo.RolAdministrativo", "EmpresaNombre", c => c.String(nullable: false, maxLength: 300));
            AlterColumn("dbo.RolAdministrativo", "EmpresaCodigo", c => c.String(nullable: false, maxLength: 100));
            CreateIndex("dbo.RolAdministrativo", "CiudadId");
        }
    }
}
