using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleA.Models {
    public class SearchBooksMaster : BindableBase {
        private readonly BooksRepository _repository = new BooksRepository();

        private int _countBook;
        public int CountBook {
            get { return _countBook; }
            set { SetProperty(ref _countBook, value); }
        }

        public ObservableCollection<Book> Books { get; private set; }

        public SearchBooksMaster() {
            this.Books = new ObservableCollection<Book>();
        }

        public async Task SearchBooksAsync(string searchString) {
            Debug.WriteLine("SearchBooksAsync() called. {0}", searchString);
            await Task.Run(() => {
                this.Books.Clear();
                var results = this._repository.SearchBooks(searchString);
                if (results != null) {
                    foreach (var book in results) {
                        this.Books.Add(book);
                        //Debug.WriteLine(book.Title);
                    }
                    this.CountBook = this.Books.Count;
                }
            });
        }
    }
}
