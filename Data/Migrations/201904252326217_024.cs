namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _024 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ImpuestoPago", new[] { "EstadoId" });
            AlterColumn("dbo.ImpuestoPago", "EstadoId", c => c.Long(nullable: false));
            CreateIndex("dbo.ImpuestoPago", "EstadoId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ImpuestoPago", new[] { "EstadoId" });
            AlterColumn("dbo.ImpuestoPago", "EstadoId", c => c.Long());
            CreateIndex("dbo.ImpuestoPago", "EstadoId");
        }
    }
}
