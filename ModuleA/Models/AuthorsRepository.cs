using System.Collections.Generic;
using System.Linq;

namespace ModuleA.Models {
    public class AuthorsRepository {
        public Author FindAuthor(int id) {
            using (var db = new BooksDbContext()) {
                var author = db.Authors.SingleOrDefault(x => x.Id == id);
                if (author == null) {
                    return null;
                }

                return new Author {
                    Id = author.Id,
                    Name = author.Name,
                    Birthday = author.Birthday,
                    Gender = author.Gender,
                    Books = author.Books
                };
            }
        }

        public IEnumerable<Author> GetAuthors() {
            using (var db = new BooksDbContext()) {
                return db.Authors
                    .OrderBy(x => x.Id).ToList()
                    .Select(x => new Author {
                        Id = x.Id,
                        Name = x.Name,
                        Birthday = x.Birthday,
                        Gender = x.Gender,
                        Books = x.Books
                    }).ToList();
            }
        }

        public void InsertAuthor(Author a) {
            using (var db = new BooksDbContext()) {
                var author = new Author {
                    Name = a.Name,
                    Birthday = a.Birthday,
                    Gender = a.Gender
                };
                db.Authors.Add(author);
                db.SaveChanges();
            }
        }

        public void DeleteAuthor(int id) {
            using (var db = new BooksDbContext()) {
                var author = db.Authors.SingleOrDefault(x => x.Id == id);
                if (author != null) {
                    db.Authors.Remove(author);
                    db.SaveChanges();
                }
            }
        }

        public void UpdateAuthor(Author a) {
            using (var db = new BooksDbContext()) {
                var author = db.Authors.Single(x => x.Id == a.Id);
                author.Name = a.Name;
                author.Birthday = a.Birthday;
                author.Gender = a.Gender;
                db.SaveChanges();
            }
        }

        public bool IsExistAuthorInBooks(int id) {
            using (var db = new BooksDbContext()) {
                return db.Books.Any(x => x.Author.Id == id);
            }
        }
    }
}
