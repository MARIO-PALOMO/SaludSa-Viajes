namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _016 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Rol", "Tipo", c => c.String(maxLength: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Rol", "Tipo");
        }
    }
}
