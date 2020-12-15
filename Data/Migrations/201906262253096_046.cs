namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _046 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FacturaCabeceraPago", "AprobacionJefeArea", c => c.String(maxLength: 100));
            AddColumn("dbo.FacturaCabeceraPago", "AprobacionSubgerenteArea", c => c.String(maxLength: 100));
            AddColumn("dbo.FacturaCabeceraPago", "AprobacionGerenteArea", c => c.String(maxLength: 100));
            AddColumn("dbo.FacturaCabeceraPago", "AprobacionVicePresidenteFinanciero", c => c.String(maxLength: 100));
            AddColumn("dbo.FacturaCabeceraPago", "AprobacionGerenteGeneral", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FacturaCabeceraPago", "AprobacionGerenteGeneral");
            DropColumn("dbo.FacturaCabeceraPago", "AprobacionVicePresidenteFinanciero");
            DropColumn("dbo.FacturaCabeceraPago", "AprobacionGerenteArea");
            DropColumn("dbo.FacturaCabeceraPago", "AprobacionSubgerenteArea");
            DropColumn("dbo.FacturaCabeceraPago", "AprobacionJefeArea");
        }
    }
}
