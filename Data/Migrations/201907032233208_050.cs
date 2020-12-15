namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _050 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AprobadoresContabilizacionPagoes", "Id", "dbo.TareaPago");
            DropIndex("dbo.AprobadoresContabilizacionPagoes", new[] { "Id" });
            DropTable("dbo.AprobadoresContabilizacionPagoes");
        }
        
        public override void Down()
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
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.AprobadoresContabilizacionPagoes", "Id");
            AddForeignKey("dbo.AprobadoresContabilizacionPagoes", "Id", "dbo.TareaPago", "Id");
        }
    }
}
