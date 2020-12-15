namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _018 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RolAdministrativo", "IX_ColaboradorUsuario_EmpresaCodigo_RolId");
            CreateIndex("dbo.RolAdministrativo", new[] { "ColaboradorUsuario", "EmpresaCodigo", "RolId" }, unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.RolAdministrativo", new[] { "ColaboradorUsuario", "EmpresaCodigo", "RolId" });
            CreateIndex("dbo.RolAdministrativo", new[] { "ColaboradorUsuario", "RolId" }, unique: true, name: "IX_ColaboradorUsuario_EmpresaCodigo_RolId");
        }
    }
}
