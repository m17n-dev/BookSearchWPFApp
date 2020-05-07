using ModuleA.Models;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.ComponentModel;
using System.Reactive.Disposables;

namespace ModuleA.ViewModels {
    public class SearchBookViewModel : BindableBase, INotifyPropertyChanged {
        private CompositeDisposable _disposable { get; } = new CompositeDisposable();
        private readonly AppContext _model = AppContext.Instance;
        public ReadOnlyReactiveCollection<BookViewModel> Books { get; private set; }
        public ReactiveProperty<string> SearchString { get; private set; }
        public ReactiveProperty<int> CountedBook { get; private set; }
        public AsyncReactiveCommand SearchCommand { get; private set; }

        public SearchBookViewModel() {
            this.SearchString = new ReactiveProperty<string>("")
                .AddTo(this._disposable);

            this.Books = this._model
                .SearchBooksMaster.Books
                .ToReadOnlyReactiveCollection(x => new BookViewModel(x));

            this.CountedBook = this._model
                .SearchBooksMaster
                .ObserveProperty(x => x.CountBook)
                .ToReactiveProperty()
                .AddTo(this._disposable);

            this.SearchCommand = new AsyncReactiveCommand();
            this.SearchCommand
                .Subscribe(async _ => await this._model
                .SearchBooksMaster.SearchBooksAsync(this.SearchString.Value));
        }
    }
}
