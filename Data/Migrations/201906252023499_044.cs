namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _044 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InformacionContabilidadPagoes",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        TipoDiarioCodigo = c.String(nullable: false),
                        TipoDiarioDescripcion = c.String(nullable: false),
                        DiarioCodigo = c.String(nullable: false),
                        DiarioDescripcion = c.String(nullable: false),
                        PerfilAsientoContableCodigo = c.String(nullable: false),
                        PerfilAsientoContableDescripcion = c.String(nullable: false),
                        DepartamentoCodigo = c.String(nullable: false),
                        DepartamentoDescripcion = c.String(nullable: false),
                        DepartamentoCodigoDescripcion = c.String(nullable: false),
                        CuentaContableCodigo = c.String(nullable: false),
                        CuentaContableNombre = c.String(nullable: false),
                        CuentaContableTipo = c.String(nullable: false),
                        EsReembolso = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TareaPago", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InformacionContabilidadPagoes", "Id", "dbo.TareaPago");
            DropIndex("dbo.InformacionContabilidadPagoes", new[] { "Id" });
            DropTable("dbo.InformacionContabilidadPagoes");
        }
    }
}
