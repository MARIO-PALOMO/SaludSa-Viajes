namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _049 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AprobadoresContabilizacionPagoes",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        AprobacionJefeArea = c.String(maxLength: 100),
                        AprobacionSubgerenteArea = c.String(maxLength: 100),
                        AprobacionGerenteArea = c.String(maxLength: 100),
                        AprobacionVicePresidenteFinanciero = c.String(maxLength: 100),
                        AprobacionGerenteGeneral = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TareaPago", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AprobadoresContabilizacionPagoes", "Id", "dbo.TareaPago");
            DropIndex("dbo.AprobadoresContabilizacionPagoes", new[] { "Id" });
            DropTable("dbo.AprobadoresContabilizacionPagoes");
        }
    }
}
