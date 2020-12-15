namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _010 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RolAdministrativo", new[] { "RolId" });
            CreateIndex("dbo.RolAdministrativo", new[] { "CoolaboradorUsuario", "EmpresaCodigo", "RolId" }, unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.RolAdministrativo", new[] { "CoolaboradorUsuario", "EmpresaCodigo", "RolId" });
            CreateIndex("dbo.RolAdministrativo", "RolId");
        }
    }
}
