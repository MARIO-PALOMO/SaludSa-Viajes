namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _037 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ComprobanteElectronico", "FacturaCabeceraPagoId", "dbo.FacturaCabeceraPago");
            DropIndex("dbo.ComprobanteElectronico", new[] { "FacturaCabeceraPagoId" });
            AddColumn("dbo.FacturaCabeceraPago", "ComprobanteElectronicoId", c => c.Long());
            CreateIndex("dbo.FacturaCabeceraPago", "ComprobanteElectronicoId");
            AddForeignKey("dbo.FacturaCabeceraPago", "ComprobanteElectronicoId", "dbo.ComprobanteElectronico", "Id");
            DropColumn("dbo.ComprobanteElectronico", "FacturaCabeceraPagoId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ComprobanteElectronico", "FacturaCabeceraPagoId", c => c.Long());
            DropForeignKey("dbo.FacturaCabeceraPago", "ComprobanteElectronicoId", "dbo.ComprobanteElectronico");
            DropIndex("dbo.FacturaCabeceraPago", new[] { "ComprobanteElectronicoId" });
            DropColumn("dbo.FacturaCabeceraPago", "ComprobanteElectronicoId");
            CreateIndex("dbo.ComprobanteElectronico", "FacturaCabeceraPagoId");
            AddForeignKey("dbo.ComprobanteElectronico", "FacturaCabeceraPagoId", "dbo.FacturaCabeceraPago", "Id");
        }
    }
}
