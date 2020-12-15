namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _017 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Rol", "Tipo", c => c.String(nullable: false, maxLength: 1));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Rol", "Tipo", c => c.String(maxLength: 1));
        }
    }
}
