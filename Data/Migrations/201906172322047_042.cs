namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _042 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FacturaDetallePago", "GrupoImpuestoCodigo", c => c.String());
            AddColumn("dbo.FacturaDetallePago", "GrupoImpuestoDescripcion", c => c.String());
            AddColumn("dbo.FacturaDetallePago", "GrupoImpuestoArticuloCodigo", c => c.String());
            AddColumn("dbo.FacturaDetallePago", "GrupoImpuestoArticuloDescripcion", c => c.String());
            AddColumn("dbo.FacturaDetallePago", "GrupoImpuestoArticuloCodigoDescripcion", c => c.String());
            AddColumn("dbo.FacturaDetallePago", "SustentoTributarioCodigo", c => c.String());
            AddColumn("dbo.FacturaDetallePago", "SustentoTributarioDescripcion", c => c.String());
            AddColumn("dbo.FacturaDetallePago", "SustentoTributarioCodigoDescripcion", c => c.String());
            AddColumn("dbo.FacturaDetallePago", "ImpuestoRentaGrupoImpuestoArticuloCodigo", c => c.String());
            AddColumn("dbo.FacturaDetallePago", "ImpuestoRentaGrupoImpuestoArticuloDescripcion", c => c.String());
            AddColumn("dbo.FacturaDetallePago", "ImpuestoRentaGrupoImpuestoArticuloCodigoDescripcion", c => c.String());
            AddColumn("dbo.FacturaDetallePago", "IvaGrupoImpuestoArticuloCodigo", c => c.String());
            AddColumn("dbo.FacturaDetallePago", "IvaGrupoImpuestoArticuloDescripcion", c => c.String());
            AddColumn("dbo.FacturaDetallePago", "IvaGrupoImpuestoArticuloCodigoDescripcion", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FacturaDetallePago", "IvaGrupoImpuestoArticuloCodigoDescripcion");
            DropColumn("dbo.FacturaDetallePago", "IvaGrupoImpuestoArticuloDescripcion");
            DropColumn("dbo.FacturaDetallePago", "IvaGrupoImpuestoArticuloCodigo");
            DropColumn("dbo.FacturaDetallePago", "ImpuestoRentaGrupoImpuestoArticuloCodigoDescripcion");
            DropColumn("dbo.FacturaDetallePago", "ImpuestoRentaGrupoImpuestoArticuloDescripcion");
            DropColumn("dbo.FacturaDetallePago", "ImpuestoRentaGrupoImpuestoArticuloCodigo");
            DropColumn("dbo.FacturaDetallePago", "SustentoTributarioCodigoDescripcion");
            DropColumn("dbo.FacturaDetallePago", "SustentoTributarioDescripcion");
            DropColumn("dbo.FacturaDetallePago", "SustentoTributarioCodigo");
            DropColumn("dbo.FacturaDetallePago", "GrupoImpuestoArticuloCodigoDescripcion");
            DropColumn("dbo.FacturaDetallePago", "GrupoImpuestoArticuloDescripcion");
            DropColumn("dbo.FacturaDetallePago", "GrupoImpuestoArticuloCodigo");
            DropColumn("dbo.FacturaDetallePago", "GrupoImpuestoDescripcion");
            DropColumn("dbo.FacturaDetallePago", "GrupoImpuestoCodigo");
        }
    }
}
