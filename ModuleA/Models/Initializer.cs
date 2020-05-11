using ModuleA.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ModuleA.Models {
    public class Initializer : DropCreateDatabaseIfModelChanges<BooksDbContext> {
        protected override void Seed(BooksDbContext context) {
            Debug.WriteLine("Initializer.Seed() called.");
            SeedAsync(context).Wait();
        }

        protected async Task SeedAsync(BooksDbContext context) {
            Debug.WriteLine("SeedAsync called().");
            await Task.Run(() => {
                var authors = new List<Author> {
                    new Author { Name = "Haruki Murakami", Birthday = DateTime.Parse("1949-01-21"), Gender = GenderType.Male },
                    new Author { Name = "Francis Scott Key Fitzgerald", Birthday = DateTime.Parse("1896-09-24"), Gender = GenderType.Male },
                    new Author { Name = "Joanne Rowling", Birthday = DateTime.Parse("1965-07-31"), Gender = GenderType.Female },
                };
                authors.ForEach(x => context.Authors.Add(x));
                context.SaveChanges();

                var publishers = new List<Publisher> {
                    new Publisher { Name = "Kodansha Ltd", Address = "2-12-21 Otowa Bunkyo-ku Tokyo, 112-8001 Japan" },
                    new Publisher { Name = "Rutgers University Press", Address = "Chicago Distribution Center 11030 South Langley Avenue, Chicago USA" },
                    new Publisher { Name = "Bloomsbury Publishing", Address = "Broadway, 5th Floor, New York, NY USA" },
                    new Publisher { Name = "Knopf Doubleday Publishing Group", Address = "1745 Broadway New York, NY USA" },
                };
                publishers.ForEach(x => context.Publishers.Add(x));
                context.SaveChanges();

                var author1 = context.Authors.Single(a => a.Name == "Haruki Murakami");
                var publisher1 = context.Publishers.Single(p => p.Name == "Kodansha Ltd");
                var author2 = context.Authors.Single(a => a.Name == "Francis Scott Key Fitzgerald");
                var publisher2 = context.Publishers.Single(p => p.Name == "Rutgers University Press");
                var author3 = context.Authors.Single(a => a.Name == "Joanne Rowling");
                var publisher3 = context.Publishers.Single(p => p.Name == "Bloomsbury Publishing");
                var books = new List<Book> {
                    new Book { Title="Hear the Wind Sing", Author=author1, PublishedYear=1987, Publisher=publisher1 },
                    new Book { Title="Norwegian Wood", Author=author1, PublishedYear=1987, Publisher=publisher1 },
                    new Book { Title="Kafka on the Shore", Author=author1, PublishedYear=2005, Publisher=publisher1 },
                    new Book { Title="The Apprentice Fiction of F. Scott Fitzgerald", Author=author2, PublishedYear=1965, Publisher=publisher2 },
                    new Book { Title="Harry Potter and the Half-Blood Prince.", Author=author3, PublishedYear=2005, Publisher=publisher3 },
                };
                books.ForEach(x => context.Books.Add(x));
                context.SaveChanges();
            });
        }
    }
}
