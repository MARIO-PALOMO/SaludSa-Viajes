namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _033 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SolicitudPagoCabecera", "MontoTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SolicitudPagoCabecera", "MontoTotal");
        }
    }
}
