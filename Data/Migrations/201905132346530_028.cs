namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _028 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FacturaDetallePagoDistribucion",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Porcentaje = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DepartamentoCodigo = c.String(nullable: false),
                        DepartamentoDescripcion = c.String(nullable: false),
                        DepartamentoCodigoDescripcion = c.String(nullable: false),
                        CentroCostoCodigo = c.String(nullable: false),
                        CentroCostoDescripcion = c.String(nullable: false),
                        CentroCostoCodigoDescripcion = c.String(nullable: false),
                        PropositoCodigo = c.String(nullable: false),
                        PropositoDescripcion = c.String(nullable: false),
                        PropositoCodigoDescripcion = c.String(nullable: false),
                        FacturaDetallePagoId = c.Long(),
                        EstadoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Estado", t => t.EstadoId)
                .ForeignKey("dbo.FacturaDetallePago", t => t.FacturaDetallePagoId)
                .Index(t => t.FacturaDetallePagoId)
                .Index(t => t.EstadoId);
            
            AddColumn("dbo.FacturaCabeceraPago", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.FacturaCabeceraPago", "TipoPagoId", c => c.Long(nullable: false));
            AddColumn("dbo.FacturaDetallePago", "Valor", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.FacturaDetallePago", "Subtotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.FacturaDetallePago", "ImpuestoPagoId", c => c.Long(nullable: false));
            AlterColumn("dbo.FacturaCabeceraPago", "NoFactura", c => c.String(nullable: false, maxLength: 15));
            AlterColumn("dbo.FacturaCabeceraPago", "NoAutorización", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.FacturaCabeceraPago", "Concepto", c => c.String(nullable: false, maxLength: 300));
            AlterColumn("dbo.FacturaCabeceraPago", "FechaEmision", c => c.DateTime(nullable: false));
            AlterColumn("dbo.FacturaCabeceraPago", "FechaVencimiento", c => c.DateTime(nullable: false));
            AlterColumn("dbo.FacturaDetallePago", "Descripcion", c => c.String(nullable: false, maxLength: 300));
            CreateIndex("dbo.FacturaCabeceraPago", "TipoPagoId");
            CreateIndex("dbo.FacturaDetallePago", "ImpuestoPagoId");
            AddForeignKey("dbo.FacturaDetallePago", "ImpuestoPagoId", "dbo.ImpuestoPago", "Id");
            AddForeignKey("dbo.FacturaCabeceraPago", "TipoPagoId", "dbo.TipoPago", "Id");
            DropColumn("dbo.FacturaCabeceraPago", "TipoPago");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FacturaCabeceraPago", "TipoPago", c => c.String());
            DropForeignKey("dbo.FacturaCabeceraPago", "TipoPagoId", "dbo.TipoPago");
            DropForeignKey("dbo.FacturaDetallePago", "ImpuestoPagoId", "dbo.ImpuestoPago");
            DropForeignKey("dbo.FacturaDetallePagoDistribucion", "FacturaDetallePagoId", "dbo.FacturaDetallePago");
            DropForeignKey("dbo.FacturaDetallePagoDistribucion", "EstadoId", "dbo.Estado");
            DropIndex("dbo.FacturaDetallePagoDistribucion", new[] { "EstadoId" });
            DropIndex("dbo.FacturaDetallePagoDistribucion", new[] { "FacturaDetallePagoId" });
            DropIndex("dbo.FacturaDetallePago", new[] { "ImpuestoPagoId" });
            DropIndex("dbo.FacturaCabeceraPago", new[] { "TipoPagoId" });
            AlterColumn("dbo.FacturaDetallePago", "Descripcion", c => c.String());
            AlterColumn("dbo.FacturaCabeceraPago", "FechaVencimiento", c => c.String());
            AlterColumn("dbo.FacturaCabeceraPago", "FechaEmision", c => c.String());
            AlterColumn("dbo.FacturaCabeceraPago", "Concepto", c => c.String());
            AlterColumn("dbo.FacturaCabeceraPago", "NoAutorización", c => c.String());
            AlterColumn("dbo.FacturaCabeceraPago", "NoFactura", c => c.String());
            DropColumn("dbo.FacturaDetallePago", "ImpuestoPagoId");
            DropColumn("dbo.FacturaDetallePago", "Subtotal");
            DropColumn("dbo.FacturaDetallePago", "Valor");
            DropColumn("dbo.FacturaCabeceraPago", "TipoPagoId");
            DropColumn("dbo.FacturaCabeceraPago", "Total");
            DropTable("dbo.FacturaDetallePagoDistribucion");
        }
    }
}
