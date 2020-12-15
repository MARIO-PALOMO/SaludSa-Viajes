namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _007 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OrdenMadreLinea", "Observacion", c => c.String(nullable: false, maxLength: 500));
            AlterColumn("dbo.Tarea", "Observacion", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tarea", "Observacion", c => c.String(maxLength: 300));
            AlterColumn("dbo.OrdenMadreLinea", "Observacion", c => c.String(nullable: false, maxLength: 300));
        }
    }
}
