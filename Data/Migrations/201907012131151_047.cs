namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _047 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DiarioCabeceraPago",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        nombreDiario = c.String(nullable: false, maxLength: 300),
                        descripcionDiario = c.String(nullable: false),
                        codigoCompania = c.String(nullable: false, maxLength: 100),
                        numeroDiario = c.String(nullable: false, maxLength: 15),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TareaPago", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.DiarioCierrePago",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        numeroDiario = c.String(nullable: false, maxLength: 15),
                        autorizacion = c.String(maxLength: 100),
                        fechaVigencia = c.String(nullable: false),
                        autorizacionElectronica = c.String(maxLength: 100),
                        codigoCompania = c.String(nullable: false, maxLength: 100),
                        cierreDiarioId = c.String(nullable: false, maxLength: 15),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DiarioCabeceraPago", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.DiarioDetalleLineaPago",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        tipoDiario = c.String(nullable: false, maxLength: 2),
                        numeroDiario = c.String(nullable: false, maxLength: 15),
                        valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        cuentaContable = c.String(maxLength: 100),
                        descripcion = c.String(nullable: false, maxLength: 300),
                        departamento = c.String(),
                        centroCosto = c.String(),
                        proposito = c.String(),
                        codigoProyecto = c.String(maxLength: 100),
                        codigoCompania = c.String(maxLength: 100),
                        parametros = c.String(),
                        credito = c.Boolean(nullable: false),
                        detalleLineaId = c.String(nullable: false, maxLength: 15),
                        DiarioCabeceraPagoId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DiarioCabeceraPago", t => t.DiarioCabeceraPagoId)
                .Index(t => t.DiarioCabeceraPagoId);
            
            CreateTable(
                "dbo.DiarioLineaProveedorPago",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        tipoDiario = c.String(nullable: false, maxLength: 2),
                        numeroDiario = c.String(nullable: false, maxLength: 15),
                        valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        fecha = c.DateTime(nullable: false),
                        proveedor = c.String(nullable: false, maxLength: 100),
                        descripcion = c.String(nullable: false),
                        referencia = c.String(maxLength: 100),
                        departameto = c.String(nullable: false),
                        perfilAsiento = c.String(nullable: false),
                        codigoCompania = c.String(nullable: false, maxLength: 100),
                        numeroFactura = c.String(maxLength: 15),
                        lineaProveedorId = c.String(nullable: false, maxLength: 15),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DiarioCabeceraPago", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DiarioCabeceraPago", "Id", "dbo.TareaPago");
            DropForeignKey("dbo.DiarioLineaProveedorPago", "Id", "dbo.DiarioCabeceraPago");
            DropForeignKey("dbo.DiarioDetalleLineaPago", "DiarioCabeceraPagoId", "dbo.DiarioCabeceraPago");
            DropForeignKey("dbo.DiarioCierrePago", "Id", "dbo.DiarioCabeceraPago");
            DropIndex("dbo.DiarioLineaProveedorPago", new[] { "Id" });
            DropIndex("dbo.DiarioDetalleLineaPago", new[] { "DiarioCabeceraPagoId" });
            DropIndex("dbo.DiarioCierrePago", new[] { "Id" });
            DropIndex("dbo.DiarioCabeceraPago", new[] { "Id" });
            DropTable("dbo.DiarioLineaProveedorPago");
            DropTable("dbo.DiarioDetalleLineaPago");
            DropTable("dbo.DiarioCierrePago");
            DropTable("dbo.DiarioCabeceraPago");
        }
    }
}
