namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _032 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FacturaCabeceraPago", "Tipo", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FacturaCabeceraPago", "Tipo");
        }
    }
}
