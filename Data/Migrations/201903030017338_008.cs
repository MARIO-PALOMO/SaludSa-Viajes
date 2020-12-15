namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _008 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RolAdministrativo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ColaboradorUsuario = c.String(nullable: false, maxLength: 100),
                        ColaboradorNombreCompleto = c.String(nullable: false, maxLength: 300),
                        ColaboradorCiudadCodigo = c.String(nullable: false, maxLength: 100),
                        EmpresaCodigo = c.String(nullable: false, maxLength: 100),
                        EmpresaNombre = c.String(nullable: false, maxLength: 300),
                        RolId = c.Long(nullable: false),
                        CiudadId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Ciudad", t => t.CiudadId)
                .ForeignKey("dbo.Rol", t => t.RolId)
                .Index(t => t.RolId)
                .Index(t => t.CiudadId);
            
            CreateTable(
                "dbo.Rol",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        Nombre = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RolAdministrativo", "RolId", "dbo.Rol");
            DropForeignKey("dbo.RolAdministrativo", "CiudadId", "dbo.Ciudad");
            DropIndex("dbo.RolAdministrativo", new[] { "CiudadId" });
            DropIndex("dbo.RolAdministrativo", new[] { "RolId" });
            DropTable("dbo.Rol");
            DropTable("dbo.RolAdministrativo");
        }
    }
}
