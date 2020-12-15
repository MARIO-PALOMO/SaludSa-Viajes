namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _004 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tarea", "CantIteraciones10Minutos", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tarea", "CantIteraciones10Minutos");
        }
    }
}
