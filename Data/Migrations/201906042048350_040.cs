namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _040 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.FacturaDetallePago", new[] { "FacturaCabeceraPago_Id" });
            RenameColumn(table: "dbo.FacturaDetallePago", name: "FacturaCabeceraPago_Id", newName: "FacturaCabeceraPagoId");
            AlterColumn("dbo.FacturaDetallePago", "FacturaCabeceraPagoId", c => c.Long(nullable: false));
            CreateIndex("dbo.FacturaDetallePago", "FacturaCabeceraPagoId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.FacturaDetallePago", new[] { "FacturaCabeceraPagoId" });
            AlterColumn("dbo.FacturaDetallePago", "FacturaCabeceraPagoId", c => c.Long());
            RenameColumn(table: "dbo.FacturaDetallePago", name: "FacturaCabeceraPagoId", newName: "FacturaCabeceraPago_Id");
            CreateIndex("dbo.FacturaDetallePago", "FacturaCabeceraPago_Id");
        }
    }
}
