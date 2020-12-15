namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _029 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FacturaDetallePago", "ImpuestoPagoId", "dbo.ImpuestoPago");
            DropPrimaryKey("dbo.ImpuestoPago");
            AlterColumn("dbo.ImpuestoPago", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.ImpuestoPago", "Id");
            AddForeignKey("dbo.FacturaDetallePago", "ImpuestoPagoId", "dbo.ImpuestoPago", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FacturaDetallePago", "ImpuestoPagoId", "dbo.ImpuestoPago");
            DropPrimaryKey("dbo.ImpuestoPago");
            AlterColumn("dbo.ImpuestoPago", "Id", c => c.Long(nullable: false));
            AddPrimaryKey("dbo.ImpuestoPago", "Id");
            AddForeignKey("dbo.FacturaDetallePago", "ImpuestoPagoId", "dbo.ImpuestoPago", "Id");
        }
    }
}
