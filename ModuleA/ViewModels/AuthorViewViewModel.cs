using ModuleA.DataTypes;
using ModuleA.Extensions;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AppContext = ModuleA.Models.AppContext;

namespace ModuleA.ViewModels {
    public class AuthorViewViewModel : BindableBase, INotifyPropertyChanged {
        private CompositeDisposable _disposable { get; } = new CompositeDisposable();
        private readonly AppContext _model = AppContext.Instance;
        public ReadOnlyReactiveCollection<AuthorViewModel> Authors { get; private set; }
        public ReactiveProperty<AuthorViewModel> InputAuthor { get; private set; }
        public ReactiveProperty<AuthorViewModel> SelectedAuthor { get; private set; }
        public ReactiveProperty<int> CountedAuthor { get; private set; }
        public InteractionRequest<Confirmation> ConfirmRequest { get; private set; }
        public InteractionRequest<INotification> EditRequest { get; private set; }
        public AsyncReactiveCommand LoadCommand { get; private set; }
        public AsyncReactiveCommand AddCommand { get; private set; }
        public ReactiveCommand DeleteCommand { get; private set; }
        public AsyncReactiveCommand EditCommand { get; private set; }
        public GenderType[] Genders { get; private set; }

        public AuthorViewViewModel() {
            this.Authors = this._model
                .AuthorsMaster.Authors
                .ToReadOnlyReactiveCollection(x => new AuthorViewModel(x));

            this.InputAuthor = this._model
                .AuthorsMaster.ObserveProperty(x => x.InputAuthor)
                .Select(x => new AuthorViewModel(x))
                .ToReactiveProperty()
                .AddTo(this._disposable);

            this.CountedAuthor = this._model
                .AuthorsMaster
                .ObserveProperty(x => x.CountAuthor)
                .ToReactiveProperty()
                .AddTo(this._disposable);

            //DatePicker Default Value
            this.InputAuthor.Value.Birthday.Value = DateTime.Now.ToString("yyyy/MM/dd");

            //ComboBox Default Value
            this.InputAuthor.Value.Gender.Value = GenderType.Male;

            this.SelectedAuthor = new ReactiveProperty<AuthorViewModel>();

            this.ConfirmRequest = new InteractionRequest<Confirmation>();
            this.EditRequest = new InteractionRequest<INotification>();

            this.LoadCommand = new AsyncReactiveCommand();
            this.LoadCommand
                .Subscribe(async _ => await this._model.AuthorsMaster.LoadAsync());

            this.AddCommand = this.InputAuthor
                .SelectMany(x => x.HasErrors)
                .Select(x => !x)
                .ToAsyncReactiveCommand();
            this.AddCommand
                .Subscribe(async _ => await this._model.AuthorsMaster.AddAuthorAsync());

            this.DeleteCommand = this.SelectedAuthor
                .Select(x => x != null)
                .ToReactiveCommand();
            this.DeleteCommand
                .SelectMany(_ => this.ConfirmRequest.RaiseAsObservable(new Confirmation {
                    Title = "Confirmation",
                    Content = "Do you want to delete?"
                }))
                .Where(x => x.Confirmed)
                .Select(_ => this.SelectedAuthor.Value.Model.Id)
                .Subscribe(async x => {
                    await this._model.AuthorsMaster.DeleteAsync(x);
                    Debug.WriteLine("Delete ... Id: {0}", x);
                });

            this.EditCommand = this.SelectedAuthor
                .Select(x => x != null)
                .ToAsyncReactiveCommand();
            this.EditCommand
                .Subscribe(async _ => {
                    await this._model.AuthorDetail.SetEditTargetAsync(this.SelectedAuthor.Value.Model.Id);
                    Debug.WriteLine("Edit ... Id: {0}", this.SelectedAuthor.Value.Model.Id);
                    this.EditRequest.Raise(new Notification { Title = "Edit" });
                });

            this.Genders = this._model.AuthorsMaster.Genders;
        }
    }
}
