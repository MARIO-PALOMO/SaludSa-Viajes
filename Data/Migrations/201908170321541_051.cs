namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _051 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ComprobanteElectronico", "estado", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ComprobanteElectronico", "estado", c => c.String());
        }
    }
}
