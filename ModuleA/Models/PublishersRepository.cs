using System.Collections.Generic;
using System.Linq;

namespace ModuleA.Models {
    public class PublishersRepository {
        public Publisher FindPublisher(int id) {
            using (var db = new BooksDbContext()) {
                var publisher = db.Publishers.SingleOrDefault(x => x.Id == id);
                if (publisher == null)
                    return null;

                return new Publisher {
                    Id = publisher.Id,
                    Name = publisher.Name,
                    Address = publisher.Address,
                    IsChecked = publisher.IsChecked
                };
            }
        }

        public IEnumerable<Publisher> GetPublishers() {
            using (var db = new BooksDbContext()) {
                return db.Publishers
                    .OrderBy(x => x.Id).ToList()
                    .Select(x => new Publisher {
                        Id = x.Id,
                        Name = x.Name,
                        Address = x.Address,
                        IsChecked = x.IsChecked,
                        Books = x.Books
                    }).ToList();
            }
        }

        public IEnumerable<string> GetPublisherNamesInBooks() {
            using (var db = new BooksDbContext()) {
                var ids = db.Publishers
                    .Where(x => x.IsChecked == true)
                    .Select(x => x.Id).ToList();
                return db.Books
                    .OrderByDescending(x => x.Id).ToList()
                    .Where(x => ids.Contains(x.Publisher.Id))
                    .Join(db.Publishers,
                        book => book.Publisher.Id,
                        publisher => publisher.Id,
                        (book, publisher) => publisher.Name)
                    .Distinct()
                    .ToList();
            }
        }

        public void InsertPublisher(Publisher p) {
            using (var db = new BooksDbContext()) {
                var publisher = new Publisher {
                    Name = p.Name,
                    Address = p.Address,
                    IsChecked = p.IsChecked
                };
                db.Publishers.Add(publisher);
                db.SaveChanges();
            }
        }

        public void DeletePublisher(int id) {
            using (var db = new BooksDbContext()) {
                var publisher = db.Publishers.SingleOrDefault(x => x.Id == id);
                if (publisher != null) {
                    db.Publishers.Remove(publisher);
                    db.SaveChanges();
                }
            }
        }

        public void DeletePublishers() {
            using (var db = new BooksDbContext()) {
                var publishers = db.Publishers.Where(x => x.IsChecked == true);
                if (publishers != null) {
                    db.Publishers.RemoveRange(publishers);
                    db.SaveChanges();
                }
            }
        }

        public void UpdatePublisher(Publisher p) {
            using (var db = new BooksDbContext()) {
                var publisher = db.Publishers.Single(x => x.Id == p.Id);
                publisher.Name = p.Name;
                publisher.Address = p.Address;
                publisher.IsChecked = p.IsChecked;
                db.SaveChanges();
            }
        }

        public void UpdateIsCheckedPublisher(Publisher p, bool isChecked) {
            using (var db = new BooksDbContext()) {
                var publisher = db.Publishers.Single(x => x.Id == p.Id);
                publisher.IsChecked = isChecked;
                db.SaveChanges();
            }
        }

        public void AllChecked() {
            using (var db = new BooksDbContext()) {
                db.Publishers.ToList().ForEach(x => x.IsChecked = true);
                db.SaveChanges();
            }
        }

        public void AllUnChecked() {
            using (var db = new BooksDbContext()) {
                db.Publishers.ToList().ForEach(x => x.IsChecked = false);
                db.SaveChanges();
            }
        }

        public bool? GetThreeState() {
            using (var db = new BooksDbContext()) {
                if (db.Publishers.All(x => x.IsChecked == true))
                    return true;
                else if (db.Publishers.All(x => x.IsChecked == false))
                    return false;
                else
                    return null;
            }
        }

        public bool HasPublisherInBooks(int id) {
            using (var db = new BooksDbContext()) {
                return db.Books.Any(x => x.Publisher.Id == id);
            }
        }
    }
}
