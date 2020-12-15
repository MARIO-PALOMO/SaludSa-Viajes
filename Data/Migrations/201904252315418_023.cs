namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _023 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Impuesto", newName: "ImpuestoPago");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.ImpuestoPago", newName: "Impuesto");
        }
    }
}
