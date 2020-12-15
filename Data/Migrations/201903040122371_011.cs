namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _011 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RolAdministrativo", new[] { "CoolaboradorUsuario", "EmpresaCodigo", "RolId" });
            AddColumn("dbo.RolAdministrativo", "ColaboradorUsuario", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.RolAdministrativo", "ColaboradorNombreCompleto", c => c.String(nullable: false, maxLength: 300));
            AddColumn("dbo.RolAdministrativo", "ColaboradorCiudadCodigo", c => c.String(nullable: false, maxLength: 100));
            CreateIndex("dbo.RolAdministrativo", new[] { "ColaboradorUsuario", "EmpresaCodigo", "RolId" }, unique: true, name: "IX_CoolaboradorUsuario_EmpresaCodigo_RolId");
            DropColumn("dbo.RolAdministrativo", "CoolaboradorUsuario");
            DropColumn("dbo.RolAdministrativo", "CoolaboradorNombreCompleto");
            DropColumn("dbo.RolAdministrativo", "CoolaboradorCiudadCodigo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RolAdministrativo", "CoolaboradorCiudadCodigo", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.RolAdministrativo", "CoolaboradorNombreCompleto", c => c.String(nullable: false, maxLength: 300));
            AddColumn("dbo.RolAdministrativo", "CoolaboradorUsuario", c => c.String(nullable: false, maxLength: 100));
            DropIndex("dbo.RolAdministrativo", "IX_CoolaboradorUsuario_EmpresaCodigo_RolId");
            DropColumn("dbo.RolAdministrativo", "ColaboradorCiudadCodigo");
            DropColumn("dbo.RolAdministrativo", "ColaboradorNombreCompleto");
            DropColumn("dbo.RolAdministrativo", "ColaboradorUsuario");
            CreateIndex("dbo.RolAdministrativo", new[] { "CoolaboradorUsuario", "EmpresaCodigo", "RolId" }, unique: true);
        }
    }
}
