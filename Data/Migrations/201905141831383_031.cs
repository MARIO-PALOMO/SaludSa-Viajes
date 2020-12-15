namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _031 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FacturaCabeceraPago", "NoAutorizacion", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.FacturaCabeceraPago", "NoAutorización");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FacturaCabeceraPago", "NoAutorización", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.FacturaCabeceraPago", "NoAutorizacion");
        }
    }
}
