namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _020 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailDestinatario",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 300),
                        Direccion = c.String(nullable: false, maxLength: 300),
                        EmailPendiente_Id = c.Long(),
                        EmailPendiente_Id1 = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmailPendiente", t => t.EmailPendiente_Id)
                .ForeignKey("dbo.EmailPendiente", t => t.EmailPendiente_Id1)
                .Index(t => t.EmailPendiente_Id)
                .Index(t => t.EmailPendiente_Id1);
            
            CreateTable(
                "dbo.EmailPendiente",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Cuerpo = c.String(),
                        Asunto = c.String(),
                        Razon = c.String(),
                        FechaRegistro = c.DateTime(nullable: false),
                        FechaEnvio = c.DateTime(),
                        UsuarioEnvio = c.String(),
                        TareaId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tarea", t => t.TareaId)
                .Index(t => t.TareaId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmailPendiente", "TareaId", "dbo.Tarea");
            DropForeignKey("dbo.EmailDestinatario", "EmailPendiente_Id1", "dbo.EmailPendiente");
            DropForeignKey("dbo.EmailDestinatario", "EmailPendiente_Id", "dbo.EmailPendiente");
            DropIndex("dbo.EmailPendiente", new[] { "TareaId" });
            DropIndex("dbo.EmailDestinatario", new[] { "EmailPendiente_Id1" });
            DropIndex("dbo.EmailDestinatario", new[] { "EmailPendiente_Id" });
            DropTable("dbo.EmailPendiente");
            DropTable("dbo.EmailDestinatario");
        }
    }
}
