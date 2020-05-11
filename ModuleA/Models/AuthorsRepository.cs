using System.Collections.Generic;
using System.Linq;

namespace ModuleA.Models {
    public class AuthorsRepository {
        public Author FindAuthor(int id) {
            using (var db = new BooksDbContext()) {
                var author = db.Authors.SingleOrDefault(x => x.Id == id);
                if (author == null)
                    return null;

                return new Author {
                    Id = author.Id,
                    Name = author.Name,
                    Birthday = author.Birthday,
                    Gender = author.Gender,
                    IsChecked = author.IsChecked,
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
                        IsChecked = x.IsChecked,
                        Books = x.Books
                    }).ToList();
            }
        }

        public void InsertAuthor(Author a) {
            using (var db = new BooksDbContext()) {
                var author = new Author {
                    Name = a.Name,
                    Birthday = a.Birthday,
                    Gender = a.Gender,
                    IsChecked = a.IsChecked
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
                author.IsChecked = a.IsChecked;
                db.SaveChanges();
            }
        }

        public void UpdateIsCheckedAuthor(Author a, bool isChecked) {
            using (var db = new BooksDbContext()) {
                var author = db.Authors.Single(x => x.Id == a.Id);
                author.IsChecked = isChecked;
                db.SaveChanges();
            }
        }

        public void AllChecked() {
            using (var db = new BooksDbContext()) {
                db.Authors.ToList().ForEach(x => x.IsChecked = true);
                db.SaveChanges();
            }
        }

        public void AllUnChecked() {
            using (var db = new BooksDbContext()) {
                db.Authors.ToList().ForEach(x => x.IsChecked = false);
                db.SaveChanges();
            }
        }

        public bool? GetThreeState() {
            using (var db = new BooksDbContext()) {
                if (db.Authors.All(x => x.IsChecked == true))
                    return true;
                else if (db.Authors.All(x => x.IsChecked == false))
                    return false;
                else
                    return null;
            }
        }

        public bool HasAuthorInBooks(int id) {
            using (var db = new BooksDbContext()) {
                return db.Books.Any(x => x.Author.Id == id);
            }
        }
    }
}
