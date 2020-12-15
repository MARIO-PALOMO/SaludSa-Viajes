namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _005 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlantillaDistribucionCabecera", "EmpresaCodigo", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlantillaDistribucionCabecera", "EmpresaCodigo");
        }
    }
}
