namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _035 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ComprobanteElectronico", "FacturaCabeceraPagoId", c => c.Long());
            CreateIndex("dbo.ComprobanteElectronico", "FacturaCabeceraPagoId");
            AddForeignKey("dbo.ComprobanteElectronico", "FacturaCabeceraPagoId", "dbo.FacturaCabeceraPago", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ComprobanteElectronico", "FacturaCabeceraPagoId", "dbo.FacturaCabeceraPago");
            DropIndex("dbo.ComprobanteElectronico", new[] { "FacturaCabeceraPagoId" });
            DropColumn("dbo.ComprobanteElectronico", "FacturaCabeceraPagoId");
        }
    }
}
