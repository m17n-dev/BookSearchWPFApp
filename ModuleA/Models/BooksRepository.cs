using System;
using System.Collections.Generic;
using System.Linq;

namespace ModuleA.Models {
    public class BooksRepository {
        public Book FindBook(int id) {
            using (var db = new BooksDbContext()) {
                var book = db.Books.SingleOrDefault(x => x.Id == id);
                if (book == null) {
                    return null;
                }
                var author = db.Authors.SingleOrDefault(a => a.Id == book.Author.Id);
                if (author == null) {
                    return null;
                }
                var publisher = db.Publishers.SingleOrDefault(p => p.Id == book.Publisher.Id);
                if (publisher == null) {
                    return null;
                }

                return new Book {
                    Id = book.Id,
                    Title = book.Title,
                    Author = author,
                    PublishedYear = book.PublishedYear,
                    Publisher = publisher
                };
            }
        }

        public IEnumerable<Book> SearchBooks(string searchString) {
            var searchTerms = searchString.ToLower().Split(null);
            using (var db = new BooksDbContext()) {
                if (searchTerms.Length == 0)
                    return null;
                return db.Books
                    .Where(book => searchTerms.Any(s => book.Title.Contains(s))).ToList()
                    .Select(x => new Book {
                        Id = x.Id,
                        Title = x.Title,
                        Author = x.Author,
                        PublishedYear = x.PublishedYear,
                        Publisher = x.Publisher
                    }).ToList();
            }
        }

        public IEnumerable<Book> GetBooks() {
            using (var db = new BooksDbContext()) {
                return db.Books
                    .OrderByDescending(x => x.PublishedYear).ToList()
                    .Select(x => new Book {
                        Id = x.Id,
                        Title = x.Title,
                        Author = x.Author,
                        PublishedYear = x.PublishedYear,
                        Publisher = x.Publisher

                    }).ToList();
            }
        }

        public List<int> GetYears() {
            //Enumerable.Range(1950, DateTime.Now.Year - 1950 + 1).ToList().ForEach(i => Debug.WriteLine(i));
            return Enumerable.Range(1950, DateTime.Now.Year - 1950 + 1).ToList();
        }

        public void InsertBook(Book b) {
            using (var db = new BooksDbContext()) {
                var author = db.Authors.Single(a => a.Id == b.Author.Id);
                var publisher = db.Publishers.Single(p => p.Id == b.Publisher.Id);
                var book = new Book {
                    Title = b.Title,
                    Author = author,
                    PublishedYear = b.PublishedYear,
                    Publisher = publisher
                };
                db.Books.Add(book);
                db.SaveChanges();
            }
        }

        public void DeleteBook(int id) {
            using (var db = new BooksDbContext()) {
                var book = db.Books.SingleOrDefault(x => x.Id == id);
                if (book != null) {
                    db.Books.Remove(book);
                    db.SaveChanges();
                }
            }
        }

        public void UpdateBook(Book b) {
            using (var db = new BooksDbContext()) {
                var book = db.Books.Single(x => x.Id == b.Id);
                var author = db.Authors.Single(a => a.Id == b.Author.Id);
                var publisher = db.Publishers.Single(p => p.Id == b.Publisher.Id);
                book.Title = b.Title;
                book.Author = author;
                book.PublishedYear = b.PublishedYear;
                book.Publisher = publisher;
                db.SaveChanges();
            }
        }
    }
}
