namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _019 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RolAdministrativo", new[] { "ColaboradorUsuario", "EmpresaCodigo", "RolId" });
            DropIndex("dbo.RolAdministrativo", new[] { "CiudadId" });
            CreateIndex("dbo.RolAdministrativo", new[] { "ColaboradorUsuario", "EmpresaCodigo", "RolId", "CiudadId" }, unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.RolAdministrativo", new[] { "ColaboradorUsuario", "EmpresaCodigo", "RolId", "CiudadId" });
            CreateIndex("dbo.RolAdministrativo", "CiudadId");
            CreateIndex("dbo.RolAdministrativo", new[] { "ColaboradorUsuario", "EmpresaCodigo", "RolId" }, unique: true);
        }
    }
}
