using System.Collections.Generic;
using System.Linq;

namespace ModuleA.Models {
    public class PublishersRepository {
        public Publisher FindPublisher(int id) {
            using (var db = new BooksDbContext()) {
                var publisher = db.Publishers.SingleOrDefault(x => x.Id == id);
                if (publisher == null) {
                    return null;
                }

                return new Publisher {
                    Id = publisher.Id,
                    Name = publisher.Name,
                    Address = publisher.Address
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
                        Books = x.Books
                    }).ToList();
            }
        }

        public void InsertPublisher(Publisher p) {
            using (var db = new BooksDbContext()) {
                var publisher = new Publisher {
                    Name = p.Name,
                    Address = p.Address
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

        public void UpdatePublisher(Publisher p) {
            using (var db = new BooksDbContext()) {
                var publisher = db.Publishers.Single(x => x.Id == p.Id);
                publisher.Name = p.Name;
                publisher.Address = p.Address;
                db.SaveChanges();
            }
        }

        public bool IsExistPublisherInBooks(int id) {
            using (var db = new BooksDbContext()) {
                return db.Books.Any(x => x.Publisher.Id == id);
            }
        }
    }
}
