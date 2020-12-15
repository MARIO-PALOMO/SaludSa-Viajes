namespace Data.Context
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Web;

    using System.Data.Entity.Validation;
    using Common.Utilities;
    using Common.ViewModels;
    using Data.Entidades;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public class ApplicationDbContext : DbContext, IDisposable
    {

        public virtual DbSet<SolicitudCompraCabecera> SolicitudesCompraCabecera { get; set; }
        public virtual DbSet<SolicitudCompraDetalle> SolicitudesCompraDetalle { get; set; }
        public virtual DbSet<PlantillaDistribucionCabecera> PlantillasDistribucionCabecera { get; set; }
        public virtual DbSet<PlantillaDistribucionDetalle> PlantillasDistribucionDetalle { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Estado> Estados { get; set; }
        public virtual DbSet<RolGestorCompra> RolesGestorCompra { get; set; }
        public virtual DbSet<SolicitudCompraDetalleDistribucion> SolicitudCompraDetalleDistribuciones { get; set; }
        public virtual DbSet<Tarea> Tareas { get; set; }
        public virtual DbSet<Ciudad> Ciudades { get; set; }
        public virtual DbSet<Region> Regiones { get; set; }
        public virtual DbSet<HistorialEmail> HistorialesEmail { get; set; }
        public virtual DbSet<OrdenMadre> OrdenesMadre { get; set; }
        public virtual DbSet<OrdenMadreLinea> OrdenMadreLineas { get; set; }
        public virtual DbSet<Recepcion> Recepciones { get; set; }
        public virtual DbSet<RecepcionLinea> RecepcionLineas { get; set; }
        public virtual DbSet<OrdenHija> OrdenesHija { get; set; }
        public virtual DbSet<OrdenHijaLinea> OrdenHijaLineas { get; set; }
        public virtual DbSet<OrdenHijaRemision> OrdenesHijaRemision { get; set; }
        public virtual DbSet<ComprobanteElectronico> ComprobantesElectronicos { get; set; }
        public virtual DbSet<ComprobanteElectronicoInfoAdicional> ComprobanteElectronicoInfosAdicional { get; set; }
        public virtual DbSet<Rol> Roles { get; set; }
        public virtual DbSet<RolAdministrativo> RolesAdministrativo { get; set; }
        public virtual DbSet<EmailPendiente> EmailsPendiente { get; set; }
        public virtual DbSet<EmailDestinatario> EmailsDestinatario { get; set; }
        public virtual DbSet<ImpuestoPago> ImpuestosPago { get; set; }
        public virtual DbSet<TipoPago> TiposPago { get; set; }
        public virtual DbSet<SolicitudPagoCabecera> SolicitudesPagoCabecera { get; set; }
        public virtual DbSet<TareaPago> TareasPago { get; set; }
        public virtual DbSet<FacturaCabeceraPago> FacturaCabecerasPago { get; set; }
        public virtual DbSet<FacturaDetallePago> FacturaDetallesPago { get; set; }
        public virtual DbSet<FacturaDetallePagoDistribucion> FacturaDetallePagoDistribuciones { get; set; }
        public virtual DbSet<InformacionContabilidadPago> InformacionesContabilidadPago { get; set; }
        public virtual DbSet<DiarioCabeceraPago> DiarioCabecerasPago { get; set; }
        public virtual DbSet<DiarioLineaProveedorPago> DiarioLineasProveedorPago { get; set; }
        public virtual DbSet<DiarioDetalleLineaPago> DiarioDetallesLineaPago { get; set; }
        public virtual DbSet<DiarioCierrePago> DiarioCierresPago { get; set; }


        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<SolicitudCompraCabecera>().ToTable("SolicitudCompraCabecera");
            modelBuilder.Entity<SolicitudCompraDetalle>().ToTable("SolicitudCompraDetalle");
            modelBuilder.Entity<PlantillaDistribucionCabecera>().ToTable("PlantillaDistribucionCabecera");
            modelBuilder.Entity<PlantillaDistribucionDetalle>().ToTable("PlantillaDistribucionDetalle");
            modelBuilder.Entity<Usuario>().ToTable("Usuario");
            modelBuilder.Entity<Estado>().ToTable("Estado");
            modelBuilder.Entity<RolGestorCompra>().ToTable("RolGestorCompra");
            modelBuilder.Entity<SolicitudCompraDetalleDistribucion>().ToTable("SolicitudCompraDetalleDistribucion");
            modelBuilder.Entity<Tarea>().ToTable("Tarea");
            modelBuilder.Entity<Ciudad>().ToTable("Ciudad");
            modelBuilder.Entity<Region>().ToTable("Region");
            modelBuilder.Entity<HistorialEmail>().ToTable("HistorialEmail");
            modelBuilder.Entity<OrdenMadre>().ToTable("OrdenMadre");
            modelBuilder.Entity<OrdenMadreLinea>().ToTable("OrdenMadreLinea");
            modelBuilder.Entity<Recepcion>().ToTable("Recepcion");
            modelBuilder.Entity<RecepcionLinea>().ToTable("RecepcionLinea");
            modelBuilder.Entity<OrdenHija>().ToTable("OrdenHija");
            modelBuilder.Entity<OrdenHijaLinea>().ToTable("OrdenHijaLinea");
            modelBuilder.Entity<OrdenHijaRemision>().ToTable("OrdenHijaRemision");
            modelBuilder.Entity<ComprobanteElectronico>().ToTable("ComprobanteElectronico");
            modelBuilder.Entity<ComprobanteElectronicoInfoAdicional>().ToTable("ComprobanteElectronicoInfoAdicional");
            modelBuilder.Entity<Rol>().ToTable("Rol");
            modelBuilder.Entity<RolAdministrativo>().ToTable("RolAdministrativo");
            modelBuilder.Entity<EmailPendiente>().ToTable("EmailPendiente");
            modelBuilder.Entity<EmailDestinatario>().ToTable("EmailDestinatario");
            modelBuilder.Entity<ImpuestoPago>().ToTable("ImpuestoPago");
            modelBuilder.Entity<TipoPago>().ToTable("TipoPago");
            modelBuilder.Entity<SolicitudPagoCabecera>().ToTable("SolicitudPagoCabecera");
            modelBuilder.Entity<TareaPago>().ToTable("TareaPago");
            modelBuilder.Entity<FacturaCabeceraPago>().ToTable("FacturaCabeceraPago");
            modelBuilder.Entity<FacturaDetallePago>().ToTable("FacturaDetallePago");
            modelBuilder.Entity<FacturaDetallePagoDistribucion>().ToTable("FacturaDetallePagoDistribucion");
            modelBuilder.Entity<InformacionContabilidadPago>().ToTable("InformacionContabilidadPago");
            modelBuilder.Entity<DiarioCabeceraPago>().ToTable("DiarioCabeceraPago");
            modelBuilder.Entity<DiarioLineaProveedorPago>().ToTable("DiarioLineaProveedorPago");
            modelBuilder.Entity<DiarioDetalleLineaPago>().ToTable("DiarioDetalleLineaPago");
            modelBuilder.Entity<DiarioCierrePago>().ToTable("DiarioCierrePago");
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Recupero los mensajes de error como una lista de string.
                var errorMessages = ex.EntityValidationErrors
                .SelectMany(x => x.ValidationErrors)
                .Select(x => x.ErrorMessage);

                // Uno la lista a un solo string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combino el mensaje de excepción original con el nuevo.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Lanzo una nueva DbEntityValidationException con el mensaje de excepción mejorado.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }
    }
}
