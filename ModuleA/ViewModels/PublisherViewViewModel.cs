using ModuleA.Extensions;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
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
        public InteractionRequest<Confirmation> ConfirmRequest { get; private set; }
        public InteractionRequest<INotification> EditRequest { get; private set; }
        public AsyncReactiveCommand LoadCommand { get; private set; }
        public AsyncReactiveCommand AddCommand { get; private set; }
        public ReactiveCommand DeleteCommand { get; private set; }
        public AsyncReactiveCommand EditCommand { get; private set; }

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

            this.SelectedPublisher = new ReactiveProperty<PublisherViewModel>();

            this.ConfirmRequest = new InteractionRequest<Confirmation>();
            this.EditRequest = new InteractionRequest<INotification>();

            this.LoadCommand = new AsyncReactiveCommand();
            this.LoadCommand
                .Subscribe(async _ => await this._model.PublishersMaster.LoadAsync());

            this.AddCommand = this.InputPublisher
                .SelectMany(x => x.HasErrors)
                .Select(x => !x)
                .ToAsyncReactiveCommand();
            this.AddCommand
                .Subscribe(async _ => await this._model.PublishersMaster.AddPublisherAsync());

            this.DeleteCommand = this.SelectedPublisher
                .Select(x => x != null)
                .ToReactiveCommand();
            this.DeleteCommand
                .SelectMany(_ => this.ConfirmRequest.RaiseAsObservable(new Confirmation {
                    Title = "Confirmation",
                    Content = "Do you want to delete?"
                }))
                .Where(x => x.Confirmed)
                .Select(_ => this.SelectedPublisher.Value.Model.Id)
                .Subscribe(async x => {
                    await this._model.PublishersMaster.DeleteAsync(x);
                });

            this.EditCommand = this.SelectedPublisher
                .Select(x => x != null)
                .ToAsyncReactiveCommand();
            this.EditCommand
                .Subscribe(async _ => {
                    await this._model.PublisherDetail.SetEditTargetAsync(this.SelectedPublisher.Value.Model.Id);
                    this.EditRequest.Raise(new Notification { Title = "Edit" });
                });
        }
    }
}
