using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations.EntityBuilders;
using Oqtane.Repository;

namespace Oqtane.Migrations.Tenant
{
    [DbContext(typeof(TenantDBContext))]
    [Migration("Tenant.02.01.01.00")]
    public class AddPageIsClickable : MultiDatabaseMigration
    {
        public AddPageIsClickable(IDatabase database) : base(database)
        {
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var pageEntityBuilder = new PageEntityBuilder(migrationBuilder, ActiveDatabase);

            pageEntityBuilder.AddBooleanColumn("IsClickable");
            pageEntityBuilder.UpdateColumn("IsClickable", "1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var pageEntityBuilder = new PageEntityBuilder(migrationBuilder, ActiveDatabase);

            pageEntityBuilder.DropColumn("IsClickable");
        }
    }
}
