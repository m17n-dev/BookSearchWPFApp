using System.Data.Entity;
using System.Diagnostics;

namespace ModuleA.Models {
    public class BooksDbContext : DbContext {
        public BooksDbContext() : base("BooksDbContext") {
            Debug.WriteLine("BooksDbContext() called.");
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BooksDbContext, Configuration>());
            Database.SetInitializer(new CreateDatabaseIfNotExists<BooksDbContext>());
            Database.SetInitializer(new Initializer());
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
    }
}
