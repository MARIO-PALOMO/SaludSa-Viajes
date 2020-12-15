namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _030 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FacturaDetallePago", "FacturaCabeceraPago_Id", "dbo.FacturaCabeceraPago");
            DropForeignKey("dbo.FacturaDetallePagoDistribucion", "FacturaDetallePagoId", "dbo.FacturaDetallePago");
            DropPrimaryKey("dbo.FacturaCabeceraPago");
            DropPrimaryKey("dbo.FacturaDetallePago");
            AlterColumn("dbo.FacturaCabeceraPago", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.FacturaDetallePago", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.FacturaCabeceraPago", "Id");
            AddPrimaryKey("dbo.FacturaDetallePago", "Id");
            AddForeignKey("dbo.FacturaDetallePago", "FacturaCabeceraPago_Id", "dbo.FacturaCabeceraPago", "Id");
            AddForeignKey("dbo.FacturaDetallePagoDistribucion", "FacturaDetallePagoId", "dbo.FacturaDetallePago", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FacturaDetallePagoDistribucion", "FacturaDetallePagoId", "dbo.FacturaDetallePago");
            DropForeignKey("dbo.FacturaDetallePago", "FacturaCabeceraPago_Id", "dbo.FacturaCabeceraPago");
            DropPrimaryKey("dbo.FacturaDetallePago");
            DropPrimaryKey("dbo.FacturaCabeceraPago");
            AlterColumn("dbo.FacturaDetallePago", "Id", c => c.Long(nullable: false));
            AlterColumn("dbo.FacturaCabeceraPago", "Id", c => c.Long(nullable: false));
            AddPrimaryKey("dbo.FacturaDetallePago", "Id");
            AddPrimaryKey("dbo.FacturaCabeceraPago", "Id");
            AddForeignKey("dbo.FacturaDetallePagoDistribucion", "FacturaDetallePagoId", "dbo.FacturaDetallePago", "Id");
            AddForeignKey("dbo.FacturaDetallePago", "FacturaCabeceraPago_Id", "dbo.FacturaCabeceraPago", "Id");
        }
    }
}
