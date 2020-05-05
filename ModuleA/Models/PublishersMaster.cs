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

        public ObservableCollection<Publisher> Publishers { get; private set; }

        public PublishersMaster(ISubject<object> interaction) {
            interaction.OfType<PublisherChanged>()
                .Subscribe(x => {
                    var publisher = Publishers.First(y => y.Id == x.Publisher.Id);
                    publisher.Name = x.Publisher.Name;
                    publisher.Address = x.Publisher.Address;
                    publisher.Books = x.Publisher.Books;
                });
            this.Publishers = new ObservableCollection<Publisher>();
        }

        public async Task LoadAsync() {
            await Task.Run(() => {
                this.Publishers.Clear();
                var results = this._repository.GetPublishers();
                results.ForEach(this.Publishers.Add);
                this.CountPublisher = this.Publishers.Count;
            });
        }

        public async Task DeleteAsync(int id) {
            await Task.Run(() => {
                if (!this._repository.IsExistPublisherInBooks(id)) {
                    this._repository.DeletePublisher(id);
                    this.Publishers.Remove(this.Publishers.Single(x => x.Id == id));
                    this.CountPublisher = this.Publishers.Count;
                }
                else {
                    var name = this.Publishers.Single(x => x.Id == id).Name;
                    MessageBox.Show($"You could not delete \"{ name }\" because  \"{ name } \" was registered in the book list. " +
                        $"To delete, delete all books of  \"{ name } \" registered in the book list and then delete  \"{ name } \".",
                        "Result",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            });
        }

        public async Task AddPublisherAsync() {
            await Task.Run(() => {
                this._repository.InsertPublisher(this.InputPublisher);
                this.Publishers.Clear();
                var results = this._repository.GetPublishers();
                results.ForEach(this.Publishers.Add);
                this.CountPublisher = this.Publishers.Count;
                this.InputPublisher = new Publisher();
            });
        }
    }
}
