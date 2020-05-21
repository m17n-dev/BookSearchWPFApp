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
    public class PublishersMaster : BindableBase {
        private readonly PublishersRepository _repository = new PublishersRepository();

        private Publisher _inputPublisher = new Publisher();
        public Publisher InputPublisher {
            get { return _inputPublisher; }
            set { SetProperty(ref _inputPublisher, value); }
        }

        private int _countPublisher;
        public int CountPublisher {
            get { return _countPublisher; }
            set { SetProperty(ref _countPublisher, value); }
        }

        private bool? _isCheckedHeader;
        public bool? IsCheckedHeader {
            get { return _isCheckedHeader; }
            set { SetProperty(ref _isCheckedHeader, value); }
        }

        public ObservableCollection<Publisher> Publishers { get; private set; }

        public PublishersMaster(ISubject<object> interaction) {
            interaction.OfType<PublisherChanged>()
                .Subscribe(x => {
                    var publisher = Publishers.First(y => y.Id == x.Publisher.Id);
                    publisher.Name = x.Publisher.Name;
                    publisher.Address = x.Publisher.Address;
                    publisher.IsChecked = x.Publisher.IsChecked;
                    publisher.Books = x.Publisher.Books;
                });
            this.Publishers = new ObservableCollection<Publisher>();
        }

        public async Task LoadAsync() {
            this.Publishers.Clear();
            await Task.Run(() => {
                this._repository.GetPublishers().ForEach(this.Publishers.Add);
            });
        }

        public async Task CountAsync() {
            await Task.Run(() => {
                this.CountPublisher = this.Publishers.Count;
            });
        }

        public async Task AddPublisherAsync() {
            await Task.Run(() => {
                this._repository.InsertPublisher(this.InputPublisher);
            });
            this.InputPublisher = new Publisher();
        }

        public async Task DeleteAsync(int id) {
            if (!this._repository.HasPublisherInBooks(id)) {
                await Task.Run(() => {
                    this._repository.DeletePublisher(id);
                });
                await Task.Run(() => {
                    this.Publishers.Remove(this.Publishers.Single(x => x.Id == id));
                });
            }
            else {
                var name = this.Publishers.Single(x => x.Id == id).Name;
                MessageBox.Show($"You could not delete \"{ name }\" because  \"{ name } \"" +
                    $" was registered in the book list. " +
                    $"To delete, delete all books of  \"{ name } \" registered" +
                    $" in the book list and then delete  \"{ name } \".",
                    "Result",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            } 
        }

        public async Task DeletePublishersAsync() {
            Debug.WriteLine("DeleteAthoursAsync() called.");
            var names = this._repository.GetPublisherNamesInBooks();
            names.ForEach(x => Debug.WriteLine("--- {0}", x));
            if (names.Count() == 0) {
                await Task.Run(() => {
                    this._repository.DeletePublishers();
                });
                await Task.Run(() => {
                    foreach (var x in this.Publishers.ToList()) {
                        if (x.IsChecked)
                            this.Publishers.Remove(x);
                    }
                });
            }
            else {
                string publisher = string.Join(Environment.NewLine, names.ToArray());
                string description = "You could not delete these publisher because " +
                    "these were registered in the book list. " +
                    "To delete, first delete all the publisher's books, then delete publisher.";
                var message = publisher + Environment.NewLine + Environment.NewLine + description;
                MessageBox.Show($"{ message }",
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
