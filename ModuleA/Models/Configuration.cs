using System.Data.Entity.Migrations;
using System.Diagnostics;

namespace ModuleA.Models {
    internal sealed class Configuration :
        DbMigrationsConfiguration<BooksDbContext> {
        public Configuration() {
            Debug.WriteLine("------ Configuration() called.");
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "ModuleA.Models.BooksDbContext";
        }
    }
}
