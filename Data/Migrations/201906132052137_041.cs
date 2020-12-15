namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _041 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TareaPago", "FacturaCabeceraPagoId", c => c.Long());
            CreateIndex("dbo.TareaPago", "FacturaCabeceraPagoId");
            AddForeignKey("dbo.TareaPago", "FacturaCabeceraPagoId", "dbo.FacturaCabeceraPago", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TareaPago", "FacturaCabeceraPagoId", "dbo.FacturaCabeceraPago");
            DropIndex("dbo.TareaPago", new[] { "FacturaCabeceraPagoId" });
            DropColumn("dbo.TareaPago", "FacturaCabeceraPagoId");
        }
    }
}
