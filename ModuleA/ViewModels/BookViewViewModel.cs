using ModuleA.Extensions;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AppContext = ModuleA.Models.AppContext;

namespace ModuleA.ViewModels {
    public class BookViewViewModel : BindableBase, INotifyPropertyChanged {
        private CompositeDisposable _disposable { get; } = new CompositeDisposable();
        private readonly AppContext _model = AppContext.Instance;
        public ReadOnlyReactiveCollection<BookViewModel> Books { get; private set; }
        public ReadOnlyReactiveCollection<int> Years { get; private set; }
        public ReactiveProperty<BookViewModel> InputBook { get; private set; }
        public ReactiveProperty<BookViewModel> SelectedBook { get; private set; }
        public ReactiveProperty<int> CountedBook { get; private set; }
        public ReactiveProperty<bool?> IsCheckedHeader { get; private set; }
        public InteractionRequest<Confirmation> ConfirmRequest { get; private set; }
        public InteractionRequest<INotification> EditRequest { get; private set; }
        public AsyncReactiveCommand LoadCommand { get; private set; }
        public AsyncReactiveCommand LoadYearsCommand { get; private set; }
        public AsyncReactiveCommand LoadAuthorsCommand { get; private set; }
        public AsyncReactiveCommand LoadPublishersCommand { get; private set; }
        public AsyncReactiveCommand AddCommand { get; private set; }
        public ReactiveCommand DeleteCommand { get; private set; }
        public AsyncReactiveCommand EditCommand { get; private set; }

        public BookViewViewModel() {
            this.Books = this._model
                .BooksMaster.Books
                .ToReadOnlyReactiveCollection(x => new BookViewModel(x));

            this.Years = this._model
                .BooksMaster.Years
                .ToReadOnlyReactiveCollection(x => x);

            this.InputBook = this._model
                .BooksMaster.ObserveProperty(x => x.InputBook)
                .Select(x => new BookViewModel(x))
                .ToReactiveProperty()
                .AddTo(this._disposable);

            this.CountedBook = this._model
                .BooksMaster
                .ObserveProperty(x => x.CountBook)
                .ToReactiveProperty()
                .AddTo(this._disposable);

            this.IsCheckedHeader = this._model
                .BooksMaster
                .ObserveProperty(x => x.IsCheckedHeader)
                .ToReactiveProperty()
                .AddTo(this._disposable);

            this.SelectedBook = new ReactiveProperty<BookViewModel>();

            this.ConfirmRequest = new InteractionRequest<Confirmation>();
            this.EditRequest = new InteractionRequest<INotification>();

            this.LoadCommand = new AsyncReactiveCommand();
            this.LoadCommand
                .Subscribe(async _ => {
                    await this._model.BooksMaster.LoadAsync();
                    await this._model.BooksMaster.CountAsync();
                    this.IsCheckedHeader.Value = await this._model.BooksMaster.ThreeStateAsync();
                }).AddTo(this._disposable);

            this.LoadYearsCommand = new AsyncReactiveCommand();
            this.LoadYearsCommand.Subscribe(_ => this._model.BooksMaster.LoadYearsAsync());

            this.LoadAuthorsCommand = new AsyncReactiveCommand();
            this.LoadAuthorsCommand
                .Subscribe(async _ => await this._model.AuthorsMaster.LoadAsync());

            this.LoadPublishersCommand = new AsyncReactiveCommand();
            this.LoadPublishersCommand
                .Subscribe(async _ => await this._model.PublishersMaster.LoadAsync());

            this.AddCommand = this.InputBook
                .SelectMany(x => x.HasErrors)
                .Select(x => !x)
                .ToAsyncReactiveCommand();
            this.AddCommand
                .Subscribe(async _ => {
                    await this._model.BooksMaster.AddBookAsync();
                    await this._model.BooksMaster.LoadAsync();
                    await this._model.BooksMaster.CountAsync();
                    this.IsCheckedHeader.Value = await this._model.BooksMaster.ThreeStateAsync();
                }).AddTo(this._disposable);

            this.DeleteCommand = this.Books
                .ObserveElementObservableProperty(x => x.IsChecked)
                .Select(_ => this.Books.Any(x => x.IsChecked.Value))
                .ToReactiveCommand();
            this.DeleteCommand
                .SelectMany(_ => this.ConfirmRequest.RaiseAsObservable(new Confirmation {
                    Title = "Confirmation",
                    Content = "Do you want to delete?"
                }))
                .Where(x => x.Confirmed)
                .Subscribe(async x => {
                    await this._model.BooksMaster.DeleteBooksAsync();
                    await this._model.BooksMaster.CountAsync();
                    this.IsCheckedHeader.Value = await this._model.BooksMaster.ThreeStateAsync();
                }).AddTo(this._disposable);

            this.EditCommand = this.Books
                .ObserveElementObservableProperty(x => x.IsChecked)
                .Select(_ => this.Books.Count(x => x.IsChecked.Value) == 1)
                .ToAsyncReactiveCommand();
            this.EditCommand
                .Subscribe(async _ => {
                    await this._model.BookDetail.SetEditTargetAsync(
                        this.Books.Single(x => x.IsChecked.Value == true).Id.Value);
                    this.EditRequest.Raise(new Notification { Title = "Edit" });
                }).AddTo(this._disposable);
        }

        private async void HeaderCheckAsync(bool? checkPath) {
            if (checkPath == true) {
                await this._model.BooksMaster.AllCheckedAsync();
                await this._model.BooksMaster.LoadAsync();
            }
            else if (checkPath == false) {
                await this._model.BooksMaster.AllUnCheckedAsync();
                await this._model.BooksMaster.LoadAsync();
            }
        }
    }
}
