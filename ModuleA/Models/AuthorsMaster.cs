using ModuleA.DataTypes.Enums;
using ModuleA.Extensions;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        private bool? _isCheckedHeader;
        public bool? IsCheckedHeader {
            get { return _isCheckedHeader; }
            set { SetProperty(ref _isCheckedHeader, value); }
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
                    author.IsChecked = x.Author.IsChecked;
                    author.Books = x.Author.Books;
                });
            this.Authors = new ObservableCollection<Author>();
            this.Genders = (GenderType[])Enum.GetValues(typeof(GenderType));
            this.IsCheckedHeader = false;
        }

        public async Task LoadAsync() {
            this.Authors.Clear();
            await Task.Run(() => {
                this._repository.GetAuthors().ForEach(this.Authors.Add);
            });
        }

        public async Task CountAsync() {
            await Task.Run(() => {
                this.CountAuthor = this.Authors.Count;
            });
        }

        public async Task AddAuthorAsync() {
            await Task.Run(() => {
                this._repository.InsertAuthor(this.InputAuthor);
            });
                this.InputAuthor = new Author();
                //DatePicker Default Value
                this.InputAuthor.Birthday = DateTime.Now;
                //ComboBox Default Value
                this.InputAuthor.Gender = GenderType.Male;
        }

        public async Task DeleteAsync(int id) {
                if (!this._repository.HasAuthorInBooks(id)) {
                    await Task.Run(() => {
                        this._repository.DeleteAuthor(id);
                    });
                    await Task.Run(() => {
                        this.Authors.Remove(this.Authors.Single(x => x.Id == id));
                    });
                }
                else {
                    var name = this.Authors.Single(x => x.Id == id).Name;
                    MessageBox.Show($"You could not delete  \"{ name } \" because  \"{ name } \" was registered in the book list. " +
                        $"To delete, delete all books of  \"{ name } \" registered in the book list and then delete  \"{ name } \".",
                        "Result",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
        }

        public async Task AllCheckedAsync() {
            Debug.WriteLine("AllCheckedAsync() called.");
            await Task.Run(() => {
                this._repository.AllChecked();
            });
        }

        public async Task AllUnCheckedAsync() {
            Debug.WriteLine("AllUnCheckedAsync() called.");
            await Task.Run(() => {
                this._repository.AllUnChecked();
            });
        }

        public async Task<bool?> ThreeStateAsync() {
            Debug.WriteLine("ThreeStateAsync() called.");
            var tcs = new TaskCompletionSource<bool?>();
            var threeState = this._repository.GetThreeState();
            tcs.TrySetResult(threeState);
            return await tcs.Task;
        }
    }
}
