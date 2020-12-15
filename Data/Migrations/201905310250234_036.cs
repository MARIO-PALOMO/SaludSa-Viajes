namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _036 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailPendiente", "TareaPagoId", c => c.Long());
            CreateIndex("dbo.EmailPendiente", "TareaPagoId");
            AddForeignKey("dbo.EmailPendiente", "TareaPagoId", "dbo.TareaPago", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmailPendiente", "TareaPagoId", "dbo.TareaPago");
            DropIndex("dbo.EmailPendiente", new[] { "TareaPagoId" });
            DropColumn("dbo.EmailPendiente", "TareaPagoId");
        }
    }
}
