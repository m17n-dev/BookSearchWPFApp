using ModuleA.Extensions;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ModuleA.Models {
    public class BooksMaster : BindableBase {
        private readonly BooksRepository _repository = new BooksRepository();

        private Book _inputBook = new Book();
        public Book InputBook {
            get { return _inputBook; }
            set { SetProperty(ref _inputBook, value); }
        }

        private int _countBook;
        public int CountBook {
            get { return _countBook; }
            set { SetProperty(ref _countBook, value); }
        }

        private bool? _isCheckedHeader;
        public bool? IsCheckedHeader {
            get { return _isCheckedHeader; }
            set { SetProperty(ref _isCheckedHeader, value); }
        }

        public ObservableCollection<Book> Books { get; private set; }
        public ObservableCollection<int> Years { get; private set; }
        public int YearsIndex { get; private set; }

        public BooksMaster(ISubject<object> interaction) {
            interaction.OfType<BookChanged>()
                .Subscribe(x => {
                    var book = Books.First(y => y.Id == x.Book.Id);
                    book.Title = x.Book.Title;
                    book.Author = x.Book.Author;
                    book.PublishedYear = x.Book.PublishedYear;
                    book.Publisher = x.Book.Publisher;
                });
            this.Books = new ObservableCollection<Book>();
            this.Years = new ObservableCollection<int>();
            this.IsCheckedHeader = false;
        }

        public async Task LoadAsync() {
            this.Books.Clear();
            await Task.Run(() => {
                this._repository.GetBooks().ForEach(this.Books.Add);
            });
        }

        public async Task CountAsync() {
            await Task.Run(() => {
                this.CountBook = this.Books.Count;
            });
        }

        public async Task LoadYearsAsync() {
            this.Years.Clear();
            await Task.Run(() => {
                this._repository.GetYears().ForEach(this.Years.Add);
            });
        }

        public async Task DeleteAsync(int id) {
            await Task.Run(() => {
                this._repository.DeleteBook(id);
            });
            await Task.Run(() => {
                this.Books.Remove(this.Books.Single(x => x.Id == id));
            });
        }

        public async Task DeleteBooksAsync() {
            await Task.Run(() => {
                this._repository.DeleteBooks();
            });
            await Task.Run(() => {
                foreach (var x in this.Books.ToList()) {
                    if (x.IsChecked)
                        this.Books.Remove(x);
                }
            });
        }

        public async Task AddBookAsync() {
            await Task.Run(() => {
                this._repository.InsertBook(this.InputBook);
                this.Books.Clear();
                var results = this._repository.GetBooks();
                results.ForEach(this.Books.Add);
                this.CountBook = this.Books.Count;
                this.InputBook = new Book();
                this.Years.Clear();
                var resultsYear = this._repository.GetYears();
                resultsYear.ForEach(this.Years.Add);
            });
        }
    }
}
