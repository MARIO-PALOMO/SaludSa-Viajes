namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _022 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Impuesto",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        Descripcion = c.String(nullable: false, maxLength: 100),
                        Porcentaje = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Compensacion = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EstadoId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Estado", t => t.EstadoId)
                .Index(t => t.EstadoId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Impuesto", "EstadoId", "dbo.Estado");
            DropIndex("dbo.Impuesto", new[] { "EstadoId" });
            DropTable("dbo.Impuesto");
        }
    }
}
