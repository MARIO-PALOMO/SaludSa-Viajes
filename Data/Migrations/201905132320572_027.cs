namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _027 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TareaPago",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Actividad = c.String(nullable: false, maxLength: 300),
                        UsuarioResponsable = c.String(nullable: false, maxLength: 100),
                        NombreCompletoResponsable = c.String(nullable: false, maxLength: 300),
                        EmailResponsable = c.String(maxLength: 100),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaProcesamiento = c.DateTime(),
                        TipoTarea = c.Int(nullable: false),
                        Accion = c.String(maxLength: 100),
                        Observacion = c.String(maxLength: 500),
                        SolicitudPagoCabeceraId = c.Long(),
                        EstadoId = c.Long(nullable: false),
                        TareaPadreId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Estado", t => t.EstadoId)
                .ForeignKey("dbo.SolicitudPagoCabecera", t => t.SolicitudPagoCabeceraId)
                .ForeignKey("dbo.TareaPago", t => t.TareaPadreId)
                .Index(t => t.UsuarioResponsable)
                .Index(t => t.SolicitudPagoCabeceraId)
                .Index(t => t.EstadoId)
                .Index(t => t.TareaPadreId);
            
            CreateTable(
                "dbo.SolicitudPagoCabecera",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        NumeroSolicitud = c.Long(),
                        FechaSolicitud = c.DateTime(),
                        SolicitanteUsuario = c.String(nullable: false, maxLength: 100),
                        SolicitanteNombreCompleto = c.String(nullable: false, maxLength: 300),
                        SolicitanteCiudadCodigo = c.String(nullable: false, maxLength: 100),
                        EmpresaCodigo = c.String(nullable: false, maxLength: 100),
                        EmpresaNombre = c.String(nullable: false, maxLength: 300),
                        NombreCorto = c.String(nullable: false, maxLength: 100),
                        Observacion = c.String(maxLength: 500),
                        BeneficiarioIdentificacion = c.String(maxLength: 100),
                        BeneficiarioTipoIdentificacion = c.String(maxLength: 20),
                        BeneficiarioNombre = c.String(maxLength: 300),
                        AprobacionJefeArea = c.String(maxLength: 100),
                        AprobacionSubgerenteArea = c.String(maxLength: 100),
                        AprobacionGerenteArea = c.String(maxLength: 100),
                        AprobacionVicePresidenteFinanciero = c.String(maxLength: 100),
                        AprobacionGerenteGeneral = c.String(maxLength: 100),
                        JsonOriginal = c.String(),
                        EstadoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Estado", t => t.EstadoId)
                .Index(t => t.EstadoId);
            
            CreateTable(
                "dbo.FacturaCabeceraPago",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        NoFactura = c.String(),
                        NoAutorización = c.String(),
                        TipoPago = c.String(),
                        Concepto = c.String(),
                        FechaEmision = c.String(),
                        FechaVencimiento = c.String(),
                        SolicitudPagoCabecera_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SolicitudPagoCabecera", t => t.SolicitudPagoCabecera_Id)
                .Index(t => t.SolicitudPagoCabecera_Id);
            
            CreateTable(
                "dbo.FacturaDetallePago",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        Descripcion = c.String(),
                        FacturaCabeceraPago_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FacturaCabeceraPago", t => t.FacturaCabeceraPago_Id)
                .Index(t => t.FacturaCabeceraPago_Id);
            
            AddColumn("dbo.HistorialEmail", "TareaPagoId", c => c.Long());
            CreateIndex("dbo.HistorialEmail", "TareaPagoId");
            AddForeignKey("dbo.HistorialEmail", "TareaPagoId", "dbo.TareaPago", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HistorialEmail", "TareaPagoId", "dbo.TareaPago");
            DropForeignKey("dbo.TareaPago", "TareaPadreId", "dbo.TareaPago");
            DropForeignKey("dbo.TareaPago", "SolicitudPagoCabeceraId", "dbo.SolicitudPagoCabecera");
            DropForeignKey("dbo.FacturaCabeceraPago", "SolicitudPagoCabecera_Id", "dbo.SolicitudPagoCabecera");
            DropForeignKey("dbo.FacturaDetallePago", "FacturaCabeceraPago_Id", "dbo.FacturaCabeceraPago");
            DropForeignKey("dbo.SolicitudPagoCabecera", "EstadoId", "dbo.Estado");
            DropForeignKey("dbo.TareaPago", "EstadoId", "dbo.Estado");
            DropIndex("dbo.FacturaDetallePago", new[] { "FacturaCabeceraPago_Id" });
            DropIndex("dbo.FacturaCabeceraPago", new[] { "SolicitudPagoCabecera_Id" });
            DropIndex("dbo.SolicitudPagoCabecera", new[] { "EstadoId" });
            DropIndex("dbo.TareaPago", new[] { "TareaPadreId" });
            DropIndex("dbo.TareaPago", new[] { "EstadoId" });
            DropIndex("dbo.TareaPago", new[] { "SolicitudPagoCabeceraId" });
            DropIndex("dbo.TareaPago", new[] { "UsuarioResponsable" });
            DropIndex("dbo.HistorialEmail", new[] { "TareaPagoId" });
            DropColumn("dbo.HistorialEmail", "TareaPagoId");
            DropTable("dbo.FacturaDetallePago");
            DropTable("dbo.FacturaCabeceraPago");
            DropTable("dbo.SolicitudPagoCabecera");
            DropTable("dbo.TareaPago");
        }
    }
}
