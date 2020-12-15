namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _026 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TipoPago", "EmpresaCodigo", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TipoPago", "EmpresaCodigo");
        }
    }
}
