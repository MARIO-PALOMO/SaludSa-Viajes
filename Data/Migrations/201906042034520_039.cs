namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _039 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.FacturaCabeceraPago", new[] { "SolicitudPagoCabecera_Id" });
            RenameColumn(table: "dbo.FacturaCabeceraPago", name: "SolicitudPagoCabecera_Id", newName: "SolicitudPagoCabeceraId");
            AlterColumn("dbo.FacturaCabeceraPago", "SolicitudPagoCabeceraId", c => c.Long(nullable: false));
            CreateIndex("dbo.FacturaCabeceraPago", "SolicitudPagoCabeceraId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.FacturaCabeceraPago", new[] { "SolicitudPagoCabeceraId" });
            AlterColumn("dbo.FacturaCabeceraPago", "SolicitudPagoCabeceraId", c => c.Long());
            RenameColumn(table: "dbo.FacturaCabeceraPago", name: "SolicitudPagoCabeceraId", newName: "SolicitudPagoCabecera_Id");
            CreateIndex("dbo.FacturaCabeceraPago", "SolicitudPagoCabecera_Id");
        }
    }
}
