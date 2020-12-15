namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _012 : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.RolAdministrativo", name: "IX_CoolaboradorUsuario_EmpresaCodigo_RolId", newName: "IX_ColaboradorUsuario_EmpresaCodigo_RolId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.RolAdministrativo", name: "IX_ColaboradorUsuario_EmpresaCodigo_RolId", newName: "IX_CoolaboradorUsuario_EmpresaCodigo_RolId");
        }
    }
}
