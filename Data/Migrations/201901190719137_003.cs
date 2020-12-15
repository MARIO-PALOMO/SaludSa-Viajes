namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _003 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SolicitudCompraCabecera", "JsonOriginal", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SolicitudCompraCabecera", "JsonOriginal");
        }
    }
}
