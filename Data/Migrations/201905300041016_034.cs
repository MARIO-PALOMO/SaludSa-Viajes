namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _034 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ComprobanteElectronico", new[] { "RecepcionId" });
            AlterColumn("dbo.ComprobanteElectronico", "RecepcionId", c => c.Long());
            CreateIndex("dbo.ComprobanteElectronico", "RecepcionId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ComprobanteElectronico", new[] { "RecepcionId" });
            AlterColumn("dbo.ComprobanteElectronico", "RecepcionId", c => c.Long(nullable: false));
            CreateIndex("dbo.ComprobanteElectronico", "RecepcionId");
        }
    }
}
