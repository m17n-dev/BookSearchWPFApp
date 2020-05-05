using ModuleA.DataTypes;
using ModuleA.Extensions;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;

namespace ModuleA.Models {
    public class AuthorsMaster : BindableBase {
        private readonly AuthorsRepository _repository = new AuthorsRepository();

        private Author _inputAuthor = new Author();
        public Author InputAuthor {
            get { return _inputAuthor; }
            set { SetProperty(ref _inputAuthor, value); }
        }

        private int _countAuthor;
        public int CountAuthor {
            get { return _countAuthor; }
            set { SetProperty(ref _countAuthor, value); }
        }

        public ObservableCollection<Author> Authors { get; private set; }
        public GenderType[] Genders { get; private set; }

        public AuthorsMaster(ISubject<object> interaction) {
            interaction.OfType<AuthorChanged>()
                .Subscribe(x => {
                    var author = Authors.First(y => y.Id == x.Author.Id);
                    author.Name = x.Author.Name;
                    author.Birthday = x.Author.Birthday;
                    author.Gender = x.Author.Gender;
                    author.Books = x.Author.Books;
                });
            this.Authors = new ObservableCollection<Author>();
            this.Genders = (GenderType[])Enum.GetValues(typeof(GenderType));
        }

        public async Task LoadAsync() {
            await Task.Run(() => {
                this.Authors.Clear();
                var results = this._repository.GetAuthors();
                results.ForEach(this.Authors.Add);
                this.CountAuthor = this.Authors.Count;
            });
        }

        public async Task DeleteAsync(int id) {
            await Task.Run(() => {
                if (!this._repository.IsExistAuthorInBooks(id)) {
                    this._repository.DeleteAuthor(id);
                    this.Authors.Remove(this.Authors.Single(x => x.Id == id));
                    this.CountAuthor = this.Authors.Count;
                }
                else {
                    var name = this.Authors.Single(x => x.Id == id).Name;
                    MessageBox.Show($"You could not delete  \"{ name } \" because  \"{ name } \" was registered in the book list. " +
                        $"To delete, delete all books of  \"{ name } \" registered in the book list and then delete  \"{ name } \".",
                        "Result",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            });
        }

        public async Task AddAuthorAsync() {
            await Task.Run(() => {
                this._repository.InsertAuthor(this.InputAuthor);
                //this.Authors.Add(InputAuthor); // Add at the end
                //this.Authors.Insert(0, InputAuthor); //Add to specified position. If it is 0, it is added to the first position.
                this.Authors.Clear();
                var results = this._repository.GetAuthors();
                results.ForEach(this.Authors.Add);
                this.CountAuthor = this.Authors.Count;
                this.InputAuthor = new Author();
                //DatePicker Default Value
                this.InputAuthor.Birthday = DateTime.Now;
                //ComboBox Default Value
                this.InputAuthor.Gender = GenderType.Male;
            });
        }
    }
}
