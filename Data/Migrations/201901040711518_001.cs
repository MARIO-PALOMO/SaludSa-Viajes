namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _001 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ciudad",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        Nombre = c.String(nullable: false, maxLength: 100),
                        Codigo = c.String(nullable: false, maxLength: 100),
                        Provincia = c.String(nullable: false, maxLength: 100),
                        Direccion = c.String(nullable: false, maxLength: 500),
                        RegionId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Region", t => t.RegionId)
                .Index(t => t.RegionId);
            
            CreateTable(
                "dbo.Region",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        Descripcion = c.String(nullable: false, maxLength: 100),
                        CodigoAuxiliar = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ComprobanteElectronicoInfoAdicional",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        nombre = c.String(),
                        valor = c.String(),
                        ComprobanteElectronicoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ComprobanteElectronico", t => t.ComprobanteElectronicoId)
                .Index(t => t.ComprobanteElectronicoId);
            
            CreateTable(
                "dbo.ComprobanteElectronico",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        baseImponibleCero = c.Decimal(nullable: false, precision: 18, scale: 2),
                        baseImponibleIva = c.Decimal(nullable: false, precision: 18, scale: 2),
                        baseSinCargos = c.Decimal(nullable: false, precision: 18, scale: 2),
                        claveAcceso = c.String(),
                        codigoImpuestoIva = c.Int(nullable: false),
                        establecimiento = c.String(),
                        estado = c.String(),
                        fechaAutorizacion = c.String(),
                        fechaEmisionRetencion = c.String(),
                        iva = c.Decimal(nullable: false, precision: 18, scale: 2),
                        numeroAutorizacion = c.String(),
                        observaciones = c.String(),
                        porcentajeRetencion = c.Decimal(nullable: false, precision: 18, scale: 2),
                        puntoEmision = c.String(),
                        razonSocial = c.String(),
                        ruc = c.String(),
                        secuencial = c.String(),
                        tipoDocumento = c.String(),
                        valorRetencion = c.Decimal(nullable: false, precision: 18, scale: 2),
                        valorTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        fechaEmision = c.String(),
                        RecepcionId = c.Long(nullable: false),
                        EstadoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Estado", t => t.EstadoId)
                .ForeignKey("dbo.Recepcion", t => t.RecepcionId)
                .Index(t => t.RecepcionId)
                .Index(t => t.EstadoId);
            
            CreateTable(
                "dbo.Estado",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        Descripcion = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Recepcion",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        NumeroRecepcion = c.Int(nullable: false),
                        FechaRecepcion = c.DateTime(nullable: false),
                        Aprobada = c.Boolean(nullable: false),
                        Contabilizada = c.Boolean(nullable: false),
                        EsperandoAutorizacionAnulacion = c.Boolean(nullable: false),
                        UsuarioCreador = c.String(),
                        OrdenMadreId = c.Long(nullable: false),
                        EstadoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Estado", t => t.EstadoId)
                .ForeignKey("dbo.OrdenMadre", t => t.OrdenMadreId)
                .Index(t => t.OrdenMadreId)
                .Index(t => t.EstadoId);
            
            CreateTable(
                "dbo.OrdenHija",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        NumeroOrdenHija = c.String(maxLength: 100),
                        OrdenMadreId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrdenMadre", t => t.OrdenMadreId)
                .ForeignKey("dbo.Recepcion", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.OrdenMadreId);
            
            CreateTable(
                "dbo.OrdenHijaLinea",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Cantidad = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Departamento = c.String(nullable: false, maxLength: 100),
                        CentroCosto = c.String(nullable: false, maxLength: 100),
                        Proposito = c.String(nullable: false, maxLength: 100),
                        PrecioUnitario = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PrecioTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrdenMadreLineaId = c.Long(nullable: false),
                        OrdenHijaId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrdenHija", t => t.OrdenHijaId)
                .ForeignKey("dbo.OrdenMadreLinea", t => t.OrdenMadreLineaId)
                .Index(t => t.OrdenMadreLineaId)
                .Index(t => t.OrdenHijaId);
            
            CreateTable(
                "dbo.OrdenMadreLinea",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        Tipo = c.String(nullable: false, maxLength: 100),
                        Secuencial = c.Long(nullable: false),
                        CodigoLinea = c.String(nullable: false, maxLength: 100),
                        Observacion = c.String(nullable: false, maxLength: 300),
                        CodigoArticulo = c.String(nullable: false, maxLength: 300),
                        Valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Cantidad = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrdenMadreId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrdenMadre", t => t.OrdenMadreId)
                .ForeignKey("dbo.SolicitudCompraDetalle", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.OrdenMadreId);
            
            CreateTable(
                "dbo.OrdenMadre",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Ruc = c.String(nullable: false, maxLength: 20),
                        NumeroOrdenMadre = c.String(nullable: false, maxLength: 100),
                        SolicitudCompraCabeceraId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SolicitudCompraCabecera", t => t.SolicitudCompraCabeceraId)
                .Index(t => t.SolicitudCompraCabeceraId);
            
            CreateTable(
                "dbo.SolicitudCompraCabecera",
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
                        ProveedorSugerido = c.String(maxLength: 300),
                        Frecuencia = c.String(nullable: false, maxLength: 30),
                        MontoEstimado = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductoMercadeoCodigo = c.String(maxLength: 100),
                        ProductoMercadeoNombre = c.String(maxLength: 300),
                        Descripcion = c.String(nullable: false, maxLength: 500),
                        AprobacionJefeArea = c.String(maxLength: 100),
                        AprobacionSubgerenteArea = c.String(maxLength: 100),
                        AprobacionGerenteArea = c.String(maxLength: 100),
                        AprobacionVicePresidenteFinanciero = c.String(maxLength: 100),
                        AprobacionGerenteGeneral = c.String(maxLength: 100),
                        EstadoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Estado", t => t.EstadoId)
                .Index(t => t.EstadoId);
            
            CreateTable(
                "dbo.SolicitudCompraDetalle",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CompraInternacional = c.Boolean(nullable: false),
                        Tipo = c.String(nullable: false, maxLength: 30),
                        Producto = c.String(maxLength: 300),
                        ProductoNombre = c.String(maxLength: 300),
                        GrupoProducto = c.String(maxLength: 300),
                        GrupoProductoNombre = c.String(maxLength: 300),
                        Observacion = c.String(maxLength: 500),
                        Cantidad = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Url = c.String(maxLength: 300),
                        Valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CodigoImpuestoVigente = c.String(maxLength: 100),
                        PorcentajeImpuestoVigente = c.Decimal(precision: 18, scale: 2),
                        DescripcionImpuestoVigente = c.String(maxLength: 100),
                        IdentificacionProveedor = c.String(maxLength: 20),
                        NombreProveedor = c.String(maxLength: 100),
                        RazonSocialProveedor = c.String(maxLength: 100),
                        TipoIdentificacionProveedor = c.String(maxLength: 10),
                        BloqueadoProveedor = c.String(maxLength: 10),
                        CorreoProveedor = c.String(maxLength: 100),
                        TelefonoProveedor = c.String(maxLength: 100),
                        DireccionProveedor = c.String(maxLength: 300),
                        SolicitudCompraCabeceraId = c.Long(),
                        EstadoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Estado", t => t.EstadoId)
                .ForeignKey("dbo.SolicitudCompraCabecera", t => t.SolicitudCompraCabeceraId)
                .Index(t => t.SolicitudCompraCabeceraId)
                .Index(t => t.EstadoId);
            
            CreateTable(
                "dbo.SolicitudCompraDetalleDistribucion",
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
                        SolicitudCompraDetalleId = c.Long(),
                        RecepcionLineaId = c.Long(),
                        EstadoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Estado", t => t.EstadoId)
                .ForeignKey("dbo.RecepcionLinea", t => t.RecepcionLineaId)
                .ForeignKey("dbo.SolicitudCompraDetalle", t => t.SolicitudCompraDetalleId)
                .Index(t => t.SolicitudCompraDetalleId)
                .Index(t => t.RecepcionLineaId)
                .Index(t => t.EstadoId);
            
            CreateTable(
                "dbo.RecepcionLinea",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Cantidad = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RecepcionId = c.Long(nullable: false),
                        SolicitudCompraDetalleId = c.Long(nullable: false),
                        EstadoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Estado", t => t.EstadoId)
                .ForeignKey("dbo.Recepcion", t => t.RecepcionId)
                .ForeignKey("dbo.SolicitudCompraDetalle", t => t.SolicitudCompraDetalleId)
                .Index(t => t.RecepcionId)
                .Index(t => t.SolicitudCompraDetalleId)
                .Index(t => t.EstadoId);
            
            CreateTable(
                "dbo.Tarea",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Actividad = c.String(nullable: false, maxLength: 300),
                        UsuarioResponsable = c.String(nullable: false, maxLength: 100),
                        NombreCompletoResponsable = c.String(nullable: false, maxLength: 300),
                        EmailResponsable = c.String(maxLength: 100),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaProcesamiento = c.DateTime(),
                        TiempoColor = c.Int(nullable: false),
                        TipoTarea = c.Int(nullable: false),
                        Accion = c.String(maxLength: 100),
                        Observacion = c.String(maxLength: 300),
                        Recordatorio1 = c.Boolean(nullable: false),
                        Recordatorio2 = c.Boolean(nullable: false),
                        Recordatorio3 = c.Boolean(nullable: false),
                        TiempoAviso = c.Boolean(nullable: false),
                        RetornaAJefeInmediato = c.Boolean(),
                        UsuarioGerenteArea = c.String(maxLength: 100),
                        UsuarioVicepresidenteFinanciero = c.String(maxLength: 100),
                        UsuarioAprobadorDesembolso = c.String(maxLength: 100),
                        SolicitudCompraCabeceraId = c.Long(),
                        EstadoId = c.Long(nullable: false),
                        TareaPadreId = c.Long(),
                        OrdenMadreId = c.Long(),
                        RecepcionId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Estado", t => t.EstadoId)
                .ForeignKey("dbo.OrdenMadre", t => t.OrdenMadreId)
                .ForeignKey("dbo.Recepcion", t => t.RecepcionId)
                .ForeignKey("dbo.SolicitudCompraCabecera", t => t.SolicitudCompraCabeceraId)
                .ForeignKey("dbo.Tarea", t => t.TareaPadreId)
                .Index(t => t.UsuarioResponsable)
                .Index(t => t.SolicitudCompraCabeceraId)
                .Index(t => t.EstadoId)
                .Index(t => t.TareaPadreId)
                .Index(t => t.OrdenMadreId)
                .Index(t => t.RecepcionId);
            
            CreateTable(
                "dbo.HistorialEmail",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Cuerpo = c.String(),
                        Asunto = c.String(),
                        Fecha = c.DateTime(nullable: false),
                        IdRequerimiento = c.String(),
                        Enviado = c.Boolean(nullable: false),
                        Respuesta = c.String(),
                        TareaId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tarea", t => t.TareaId)
                .Index(t => t.TareaId);
            
            CreateTable(
                "dbo.OrdenHijaRemision",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        NumeroRemisionOrdenHija = c.String(maxLength: 100),
                        RespuestaRemisionOrdenHija = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrdenHija", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.PlantillaDistribucionCabecera",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Descripcion = c.String(nullable: false, maxLength: 100),
                        UsuarioPropietario = c.String(nullable: false, maxLength: 100),
                        DescripcionDepartamentoPropietario = c.String(nullable: false, maxLength: 300),
                        EstadoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Estado", t => t.EstadoId)
                .Index(t => t.EstadoId);
            
            CreateTable(
                "dbo.PlantillaDistribucionDetalle",
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
                        DistribucionCabeceraId = c.Long(),
                        EstadoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PlantillaDistribucionCabecera", t => t.DistribucionCabeceraId)
                .ForeignKey("dbo.Estado", t => t.EstadoId)
                .Index(t => t.DistribucionCabeceraId)
                .Index(t => t.EstadoId);
            
            CreateTable(
                "dbo.RolGestorCompra",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CodigoEmpresa = c.String(nullable: false, maxLength: 100),
                        NombreEmpresa = c.String(nullable: false, maxLength: 255),
                        CodigoTipoCompra = c.String(nullable: false, maxLength: 100),
                        NombreTipoCompra = c.String(nullable: false, maxLength: 255),
                        UsuarioGestorSierra = c.String(nullable: false, maxLength: 100),
                        NombreGestorSierra = c.String(nullable: false, maxLength: 255),
                        UsuarioGestorCosta = c.String(nullable: false, maxLength: 100),
                        NombreGestorCosta = c.String(nullable: false, maxLength: 255),
                        UsuarioGestorAustro = c.String(nullable: false, maxLength: 100),
                        NombreGestorAustro = c.String(nullable: false, maxLength: 255),
                        EstadoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Estado", t => t.EstadoId)
                .Index(t => t.EstadoId);
            
            CreateTable(
                "dbo.Usuario",
                c => new
                    {
                        NombreUsuario = c.String(nullable: false, maxLength: 128),
                        EnLaOficina = c.Boolean(nullable: false),
                        UsuarioReasignar = c.String(maxLength: 100),
                        EstadoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.NombreUsuario)
                .ForeignKey("dbo.Estado", t => t.EstadoId)
                .Index(t => t.EstadoId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Usuario", "EstadoId", "dbo.Estado");
            DropForeignKey("dbo.RolGestorCompra", "EstadoId", "dbo.Estado");
            DropForeignKey("dbo.PlantillaDistribucionCabecera", "EstadoId", "dbo.Estado");
            DropForeignKey("dbo.PlantillaDistribucionDetalle", "EstadoId", "dbo.Estado");
            DropForeignKey("dbo.PlantillaDistribucionDetalle", "DistribucionCabeceraId", "dbo.PlantillaDistribucionCabecera");
            DropForeignKey("dbo.ComprobanteElectronicoInfoAdicional", "ComprobanteElectronicoId", "dbo.ComprobanteElectronico");
            DropForeignKey("dbo.ComprobanteElectronico", "RecepcionId", "dbo.Recepcion");
            DropForeignKey("dbo.Recepcion", "OrdenMadreId", "dbo.OrdenMadre");
            DropForeignKey("dbo.OrdenHija", "Id", "dbo.Recepcion");
            DropForeignKey("dbo.OrdenHija", "OrdenMadreId", "dbo.OrdenMadre");
            DropForeignKey("dbo.OrdenHijaRemision", "Id", "dbo.OrdenHija");
            DropForeignKey("dbo.OrdenHijaLinea", "OrdenMadreLineaId", "dbo.OrdenMadreLinea");
            DropForeignKey("dbo.OrdenMadreLinea", "Id", "dbo.SolicitudCompraDetalle");
            DropForeignKey("dbo.OrdenMadreLinea", "OrdenMadreId", "dbo.OrdenMadre");
            DropForeignKey("dbo.OrdenMadre", "SolicitudCompraCabeceraId", "dbo.SolicitudCompraCabecera");
            DropForeignKey("dbo.Tarea", "TareaPadreId", "dbo.Tarea");
            DropForeignKey("dbo.Tarea", "SolicitudCompraCabeceraId", "dbo.SolicitudCompraCabecera");
            DropForeignKey("dbo.Tarea", "RecepcionId", "dbo.Recepcion");
            DropForeignKey("dbo.Tarea", "OrdenMadreId", "dbo.OrdenMadre");
            DropForeignKey("dbo.HistorialEmail", "TareaId", "dbo.Tarea");
            DropForeignKey("dbo.Tarea", "EstadoId", "dbo.Estado");
            DropForeignKey("dbo.SolicitudCompraCabecera", "EstadoId", "dbo.Estado");
            DropForeignKey("dbo.SolicitudCompraDetalle", "SolicitudCompraCabeceraId", "dbo.SolicitudCompraCabecera");
            DropForeignKey("dbo.SolicitudCompraDetalle", "EstadoId", "dbo.Estado");
            DropForeignKey("dbo.SolicitudCompraDetalleDistribucion", "SolicitudCompraDetalleId", "dbo.SolicitudCompraDetalle");
            DropForeignKey("dbo.SolicitudCompraDetalleDistribucion", "RecepcionLineaId", "dbo.RecepcionLinea");
            DropForeignKey("dbo.RecepcionLinea", "SolicitudCompraDetalleId", "dbo.SolicitudCompraDetalle");
            DropForeignKey("dbo.RecepcionLinea", "RecepcionId", "dbo.Recepcion");
            DropForeignKey("dbo.RecepcionLinea", "EstadoId", "dbo.Estado");
            DropForeignKey("dbo.SolicitudCompraDetalleDistribucion", "EstadoId", "dbo.Estado");
            DropForeignKey("dbo.OrdenHijaLinea", "OrdenHijaId", "dbo.OrdenHija");
            DropForeignKey("dbo.Recepcion", "EstadoId", "dbo.Estado");
            DropForeignKey("dbo.ComprobanteElectronico", "EstadoId", "dbo.Estado");
            DropForeignKey("dbo.Ciudad", "RegionId", "dbo.Region");
            DropIndex("dbo.Usuario", new[] { "EstadoId" });
            DropIndex("dbo.RolGestorCompra", new[] { "EstadoId" });
            DropIndex("dbo.PlantillaDistribucionDetalle", new[] { "EstadoId" });
            DropIndex("dbo.PlantillaDistribucionDetalle", new[] { "DistribucionCabeceraId" });
            DropIndex("dbo.PlantillaDistribucionCabecera", new[] { "EstadoId" });
            DropIndex("dbo.OrdenHijaRemision", new[] { "Id" });
            DropIndex("dbo.HistorialEmail", new[] { "TareaId" });
            DropIndex("dbo.Tarea", new[] { "RecepcionId" });
            DropIndex("dbo.Tarea", new[] { "OrdenMadreId" });
            DropIndex("dbo.Tarea", new[] { "TareaPadreId" });
            DropIndex("dbo.Tarea", new[] { "EstadoId" });
            DropIndex("dbo.Tarea", new[] { "SolicitudCompraCabeceraId" });
            DropIndex("dbo.Tarea", new[] { "UsuarioResponsable" });
            DropIndex("dbo.RecepcionLinea", new[] { "EstadoId" });
            DropIndex("dbo.RecepcionLinea", new[] { "SolicitudCompraDetalleId" });
            DropIndex("dbo.RecepcionLinea", new[] { "RecepcionId" });
            DropIndex("dbo.SolicitudCompraDetalleDistribucion", new[] { "EstadoId" });
            DropIndex("dbo.SolicitudCompraDetalleDistribucion", new[] { "RecepcionLineaId" });
            DropIndex("dbo.SolicitudCompraDetalleDistribucion", new[] { "SolicitudCompraDetalleId" });
            DropIndex("dbo.SolicitudCompraDetalle", new[] { "EstadoId" });
            DropIndex("dbo.SolicitudCompraDetalle", new[] { "SolicitudCompraCabeceraId" });
            DropIndex("dbo.SolicitudCompraCabecera", new[] { "EstadoId" });
            DropIndex("dbo.OrdenMadre", new[] { "SolicitudCompraCabeceraId" });
            DropIndex("dbo.OrdenMadreLinea", new[] { "OrdenMadreId" });
            DropIndex("dbo.OrdenMadreLinea", new[] { "Id" });
            DropIndex("dbo.OrdenHijaLinea", new[] { "OrdenHijaId" });
            DropIndex("dbo.OrdenHijaLinea", new[] { "OrdenMadreLineaId" });
            DropIndex("dbo.OrdenHija", new[] { "OrdenMadreId" });
            DropIndex("dbo.OrdenHija", new[] { "Id" });
            DropIndex("dbo.Recepcion", new[] { "EstadoId" });
            DropIndex("dbo.Recepcion", new[] { "OrdenMadreId" });
            DropIndex("dbo.ComprobanteElectronico", new[] { "EstadoId" });
            DropIndex("dbo.ComprobanteElectronico", new[] { "RecepcionId" });
            DropIndex("dbo.ComprobanteElectronicoInfoAdicional", new[] { "ComprobanteElectronicoId" });
            DropIndex("dbo.Ciudad", new[] { "RegionId" });
            DropTable("dbo.Usuario");
            DropTable("dbo.RolGestorCompra");
            DropTable("dbo.PlantillaDistribucionDetalle");
            DropTable("dbo.PlantillaDistribucionCabecera");
            DropTable("dbo.OrdenHijaRemision");
            DropTable("dbo.HistorialEmail");
            DropTable("dbo.Tarea");
            DropTable("dbo.RecepcionLinea");
            DropTable("dbo.SolicitudCompraDetalleDistribucion");
            DropTable("dbo.SolicitudCompraDetalle");
            DropTable("dbo.SolicitudCompraCabecera");
            DropTable("dbo.OrdenMadre");
            DropTable("dbo.OrdenMadreLinea");
            DropTable("dbo.OrdenHijaLinea");
            DropTable("dbo.OrdenHija");
            DropTable("dbo.Recepcion");
            DropTable("dbo.Estado");
            DropTable("dbo.ComprobanteElectronico");
            DropTable("dbo.ComprobanteElectronicoInfoAdicional");
            DropTable("dbo.Region");
            DropTable("dbo.Ciudad");
        }
    }
}
