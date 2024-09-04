using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureHangfire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "Taggloo4.Web.Data.hangfire-install.sql";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                migrationBuilder.Sql(result);    
            }
            
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("AggregatedCounter", "Hangfire");
            migrationBuilder.DropTable("Counter", "Hangfire");
            migrationBuilder.DropTable("Hash", "Hangfire");
            migrationBuilder.DropTable("Job", "Hangfire");
            migrationBuilder.DropTable("JobQueue", "Hangfire");
            migrationBuilder.DropTable("List", "Hangfire");
            migrationBuilder.DropTable("Schema", "Hangfire");
            migrationBuilder.DropTable("Server", "Hangfire");
            migrationBuilder.DropTable("Set", "Hangfire");
            migrationBuilder.DropTable("State", "Hangfire");
        }
    }
}
