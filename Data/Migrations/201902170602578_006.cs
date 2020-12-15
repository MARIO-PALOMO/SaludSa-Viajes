namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _006 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PlantillaDistribucionCabecera", "EmpresaCodigo", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PlantillaDistribucionCabecera", "EmpresaCodigo", c => c.String(maxLength: 100));
        }
    }
}
