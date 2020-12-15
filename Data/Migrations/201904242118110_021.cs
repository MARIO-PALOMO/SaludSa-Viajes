namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _021 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.EmailDestinatario", name: "EmailPendiente_Id", newName: "EmailPendienteCopiaId");
            RenameColumn(table: "dbo.EmailDestinatario", name: "EmailPendiente_Id1", newName: "EmailPendienteDestinatarioId");
            RenameIndex(table: "dbo.EmailDestinatario", name: "IX_EmailPendiente_Id1", newName: "IX_EmailPendienteDestinatarioId");
            RenameIndex(table: "dbo.EmailDestinatario", name: "IX_EmailPendiente_Id", newName: "IX_EmailPendienteCopiaId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.EmailDestinatario", name: "IX_EmailPendienteCopiaId", newName: "IX_EmailPendiente_Id");
            RenameIndex(table: "dbo.EmailDestinatario", name: "IX_EmailPendienteDestinatarioId", newName: "IX_EmailPendiente_Id1");
            RenameColumn(table: "dbo.EmailDestinatario", name: "EmailPendienteDestinatarioId", newName: "EmailPendiente_Id1");
            RenameColumn(table: "dbo.EmailDestinatario", name: "EmailPendienteCopiaId", newName: "EmailPendiente_Id");
        }
    }
}
