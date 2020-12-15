namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _052 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ComprobanteElectronico", "numeroDocumento", c => c.String());
            AddColumn("dbo.ComprobanteElectronico", "tipoDocumentoNombre", c => c.String());
            AddColumn("dbo.ComprobanteElectronico", "estadoNombre", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ComprobanteElectronico", "estadoNombre");
            DropColumn("dbo.ComprobanteElectronico", "tipoDocumentoNombre");
            DropColumn("dbo.ComprobanteElectronico", "numeroDocumento");
        }
    }
}
