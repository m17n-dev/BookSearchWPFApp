using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AppContext = ModuleA.Models.AppContext;

namespace ModuleA.ViewModels {
    public class PublisherEditViewViewModel : IInteractionRequestAware {
        private CompositeDisposable _disposable { get; } = new CompositeDisposable();
        // Model 
        private readonly AppContext _model = AppContext.Instance;
        // IInteractionRequestAware 
        public Action FinishInteraction { get; set; }
        public INotification Notification { get; set; }
        // ---
        public ReactiveProperty<PublisherViewModel> EditTarget { get; private set; }
        public ReactiveCommand CommitCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        public PublisherEditViewViewModel() {
            this.EditTarget = this._model.PublisherDetail
                .ObserveProperty(x => x.EditTarget)
                .Where(x => x != null)
                .Select(x => new PublisherViewModel(x))
                .ToReactiveProperty()
                .AddTo(this._disposable);

            this.CommitCommand = this.EditTarget
                .Where(x => x != null)
                .SelectMany(x => x.HasErrors)
                .Select(x => !x)
                .ToReactiveCommand();
            this.CommitCommand.Subscribe(async _ => {
                await this._model.PublisherDetail.UpdateAsync();
                this.FinishInteraction();
            });

            this.CancelCommand = new DelegateCommand(CancelInteraction);
        }

        private void CancelInteraction() {
            this.FinishInteraction?.Invoke();
        }
    }
}
