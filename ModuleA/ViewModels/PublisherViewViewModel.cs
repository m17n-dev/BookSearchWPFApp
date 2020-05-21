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
    public class PublisherViewViewModel : BindableBase, INotifyPropertyChanged {
        private CompositeDisposable _disposable { get; } = new CompositeDisposable();
        private readonly AppContext _model = AppContext.Instance;
        public ReadOnlyReactiveCollection<PublisherViewModel> Publishers { get; private set; }
        public ReactiveProperty<PublisherViewModel> InputPublisher { get; private set; }
        public ReactiveProperty<PublisherViewModel> SelectedPublisher { get; private set; }
        public ReactiveProperty<int> CountedPublisher { get; private set; }
        public ReactiveProperty<bool?> IsCheckedHeader { get; private set; }
        public InteractionRequest<Confirmation> ConfirmRequest { get; private set; }
        public InteractionRequest<INotification> EditRequest { get; private set; }
        public AsyncReactiveCommand LoadCommand { get; private set; }
        public AsyncReactiveCommand AddCommand { get; private set; }
        public ReactiveCommand DeleteCommand { get; private set; }
        public AsyncReactiveCommand EditCommand { get; private set; }
        public DelegateCommand<bool?> HeaderCheckCommand { get; private set; }
        public DelegateCommand<object> CheckCommand { get; private set; }

        public PublisherViewViewModel() {
            this.Publishers = this._model
                .PublishersMaster.Publishers
                .ToReadOnlyReactiveCollection(x => new PublisherViewModel(x));

            this.InputPublisher = this._model
                .PublishersMaster.ObserveProperty(x => x.InputPublisher)
                .Select(x => new PublisherViewModel(x))
                .ToReactiveProperty()
                .AddTo(this._disposable);

            this.CountedPublisher = this._model
                .PublishersMaster
                .ObserveProperty(x => x.CountPublisher)
                .ToReactiveProperty()
                .AddTo(this._disposable);

            this.IsCheckedHeader = this._model
                .AuthorsMaster
                .ObserveProperty(x => x.IsCheckedHeader)
                .ToReactiveProperty()
                .AddTo(this._disposable);

            this.ConfirmRequest = new InteractionRequest<Confirmation>();
            this.EditRequest = new InteractionRequest<INotification>();

            this.LoadCommand = new AsyncReactiveCommand();
            this.LoadCommand
                .Subscribe(async _ => {
                    await this._model.PublishersMaster.LoadAsync();
                    await this._model.PublishersMaster.CountAsync();
                    this.IsCheckedHeader.Value = await this._model.PublishersMaster.ThreeStateAsync();
                }).AddTo(this._disposable);

            this.AddCommand = this.InputPublisher
                .SelectMany(x => x.HasErrors)
                .Select(x => !x)
                .ToAsyncReactiveCommand();
            this.AddCommand
                .Subscribe(async _ => {
                    await this._model.PublishersMaster.AddPublisherAsync();
                    await this._model.PublishersMaster.LoadAsync();
                    await this._model.PublishersMaster.CountAsync();
                    this.IsCheckedHeader.Value = await this._model.PublishersMaster.ThreeStateAsync();
                }).AddTo(this._disposable);

            this.DeleteCommand = this.Publishers
                .ObserveElementObservableProperty(x => x.IsChecked)
                .Select(_ => this.Publishers.Any(x => x.IsChecked.Value))
                .ToReactiveCommand();
            this.DeleteCommand
                .SelectMany(_ => this.ConfirmRequest.RaiseAsObservable(new Confirmation {
                    Title = "Confirmation",
                    Content = "Do you want to delete?"
                }))
                .Where(x => x.Confirmed)
                .Subscribe(async x => {
                    await this._model.PublishersMaster.DeletePublishersAsync();
                    await this._model.PublishersMaster.CountAsync();
                    this.IsCheckedHeader.Value = await this._model.PublishersMaster.ThreeStateAsync();
                }).AddTo(this._disposable);

            this.EditCommand = this.Publishers
                .ObserveElementObservableProperty(x => x.IsChecked)
                .Select(_ => this.Publishers.Count(x => x.IsChecked.Value) == 1)
                .ToAsyncReactiveCommand();
            this.EditCommand
                .Subscribe(async _ => {
                    await this._model.PublisherDetail.SetEditTargetAsync(
                        this.Publishers.Single(x => x.IsChecked.Value == true).Id.Value);
                    this.EditRequest.Raise(new Notification { Title = "Edit" });
                }).AddTo(this._disposable);

            this.HeaderCheckCommand = new DelegateCommand<bool?>(HeaderCheckAsync);

            this.CheckCommand = new DelegateCommand<object>(CheckAsync);
        }

        private async void HeaderCheckAsync(bool? checkPath) {
            Debug.WriteLine("HeaderCheckAsync() called. {0}", checkPath);
            if (checkPath == true) {
                await this._model.PublishersMaster.AllCheckedAsync();
                await this._model.PublishersMaster.LoadAsync();
            }
            else if (checkPath == false) {
                await this._model.PublishersMaster.AllUnCheckedAsync();
                await this._model.PublishersMaster.LoadAsync();
            }
        }

        private async void CheckAsync(object parameter) {
            Debug.WriteLine("CheckAsync() called. {0}", parameter);
            var values = (object[])parameter;
            var isChecked = (bool)values[0];
            var id = (int)values[1];
            Debug.WriteLine("isChecked:{0} id:{1}", isChecked, id);
            await this._model.PublisherDetail.UpdateIsCheckedAsync(isChecked, id);
            this.IsCheckedHeader.Value = await this._model.PublishersMaster.ThreeStateAsync();
            await this._model.PublishersMaster.LoadAsync();
        }
    }
}
