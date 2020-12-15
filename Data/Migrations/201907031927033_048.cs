namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _048 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DiarioCierrePago", "fechaVigencia", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DiarioCierrePago", "fechaVigencia", c => c.String(nullable: false));
        }
    }
}
