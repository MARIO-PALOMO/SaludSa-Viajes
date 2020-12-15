namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _045 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.InformacionContabilidadPagoes", newName: "InformacionContabilidadPago");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.InformacionContabilidadPago", newName: "InformacionContabilidadPagoes");
        }
    }
}
