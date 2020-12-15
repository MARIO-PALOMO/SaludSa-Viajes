namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _043 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FacturaCabeceraPago", "NoLiquidacion", c => c.String(maxLength: 15));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FacturaCabeceraPago", "NoLiquidacion");
        }
    }
}
