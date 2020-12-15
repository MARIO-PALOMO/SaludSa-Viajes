namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _025 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TipoPago",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CuentaContableCodigo = c.String(nullable: false, maxLength: 300),
                        CuentaContableNombre = c.String(nullable: false, maxLength: 300),
                        CuentaContableTipo = c.String(nullable: false, maxLength: 300),
                        Referencia = c.String(nullable: false, maxLength: 100),
                        EsReembolso = c.Boolean(nullable: false),
                        EstadoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Estado", t => t.EstadoId)
                .Index(t => t.Referencia, unique: true)
                .Index(t => t.EstadoId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TipoPago", "EstadoId", "dbo.Estado");
            DropIndex("dbo.TipoPago", new[] { "EstadoId" });
            DropIndex("dbo.TipoPago", new[] { "Referencia" });
            DropTable("dbo.TipoPago");
        }
    }
}
