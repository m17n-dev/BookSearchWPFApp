using ModuleA.DataTypes.Enums;
using ModuleA.Extensions;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
        public ReactiveProperty<bool?> IsCheckedHeader { get; private set; }
        public InteractionRequest<Confirmation> ConfirmRequest { get; private set; }
        public InteractionRequest<INotification> EditRequest { get; private set; }
        public AsyncReactiveCommand LoadCommand { get; private set; }
        public AsyncReactiveCommand AddCommand { get; private set; }
        public ReactiveCommand DeleteMultipleCommand { get; private set; }
        public AsyncReactiveCommand EditCommand { get; private set; }
        public DelegateCommand<bool?> HeaderCheckCommand { get; private set; }
        public DelegateCommand<object> CheckCommand { get; private set; }
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

            this.IsCheckedHeader = this._model
                .AuthorsMaster
                .ObserveProperty(x => x.IsCheckedHeader)
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
                .Subscribe(async _ => {
                    await this._model.AuthorsMaster.LoadAsync();
                    await this._model.AuthorsMaster.CountAsync();
                    this.IsCheckedHeader.Value = await this._model.AuthorsMaster.ThreeStateAsync();
                }).AddTo(this._disposable);

            this.AddCommand = this.InputAuthor
                .SelectMany(x => x.HasErrors)
                .Select(x => !x)
                .ToAsyncReactiveCommand();
            this.AddCommand
                .Subscribe(async _ => {
                    await this._model.AuthorsMaster.AddAuthorAsync();
                    await this._model.AuthorsMaster.LoadAsync();
                    await this._model.AuthorsMaster.CountAsync();
                    this.IsCheckedHeader.Value = await this._model.AuthorsMaster.ThreeStateAsync();
                }).AddTo(this._disposable);

            this.DeleteMultipleCommand = this.Authors
                .ObserveElementObservableProperty(x => x.IsChecked)
                .Select(_ => this.Authors.Any(x => x.IsChecked.Value))
                .ToReactiveCommand();
            this.DeleteMultipleCommand
                .SelectMany(_ => this.ConfirmRequest.RaiseAsObservable(new Confirmation {
                    Title = "Confirmation",
                    Content = "Do you want to delete?"
                }))
                .Where(x => x.Confirmed)
                .Subscribe(async x => {
                    await this._model.AuthorsMaster.DeleteAthoursAsync();
                    await this._model.AuthorsMaster.CountAsync();
                    this.IsCheckedHeader.Value = await this._model.AuthorsMaster.ThreeStateAsync();
                }).AddTo(this._disposable);

            this.EditCommand = this.SelectedAuthor
                .Select(x => x != null)
                .ToAsyncReactiveCommand();
            this.EditCommand
                .Subscribe(async _ => {
                    await this._model.AuthorDetail.SetEditTargetAsync(this.SelectedAuthor.Value.Model.Id);
                    this.EditRequest.Raise(new Notification { Title = "Edit" });
                }).AddTo(this._disposable);

            this.HeaderCheckCommand = new DelegateCommand<bool?>(HeaderCheckAsync);

            this.CheckCommand = new DelegateCommand<object>(CheckAsync);

            this.Genders = this._model.AuthorsMaster.Genders;
        }

        private async void HeaderCheckAsync(bool? checkPath) {
            Debug.WriteLine("HeaderCheckAsync() called. {0}", checkPath);
            if (checkPath == true) {
                await this._model.AuthorsMaster.AllCheckedAsync();
                await this._model.AuthorsMaster.LoadAsync();
            }
            else if (checkPath == false) {
                await this._model.AuthorsMaster.AllUnCheckedAsync();
                await this._model.AuthorsMaster.LoadAsync();
            }
        }

        private async void CheckAsync(object parameter) {
            Debug.WriteLine("CheckAsync() called. {0}", parameter);
            var values = (object[])parameter;
            var isChecked = (bool)values[0];
            var id = (int)values[1];
            Debug.WriteLine("isChecked:{0} id:{1}", isChecked, id);
            await this._model.AuthorDetail.UpdateIsCheckedAsync(isChecked, id);
            this.IsCheckedHeader.Value = await this._model.AuthorsMaster.ThreeStateAsync();
            await this._model.AuthorsMaster.LoadAsync();
        }
    }
}
