using ModuleA.DataTypes;
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
    public class AuthorEditViewViewModel : IInteractionRequestAware {
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        // Model 
        private readonly AppContext _model = AppContext.Instance;

        // IInteractionRequestAware 
        public Action FinishInteraction { get; set; }
        public INotification Notification { get; set; }

        public ReactiveProperty<AuthorViewModel> EditTarget { get; private set; }
        public ReactiveCommand CommitCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public GenderType[] Genders { get; private set; }

        public AuthorEditViewViewModel() {
            this.EditTarget = this._model.AuthorDetail
                .ObserveProperty(x => x.EditTarget)
                .Where(x => x != null)
                .Select(x => new AuthorViewModel(x))
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.CommitCommand = this.EditTarget
                .Where(x => x != null)
                .SelectMany(x => x.HasErrors)
                .Select(x => !x)
                .ToReactiveCommand();
            this.CommitCommand.Subscribe(async _ => {
                await this._model.AuthorDetail.UpdateAsync();
                this.FinishInteraction();
            });

            this.CancelCommand = new DelegateCommand(CancelInteraction);

            this.Genders = this._model.AuthorsMaster.Genders;
        }

        private void CancelInteraction() {
            this.FinishInteraction?.Invoke();
        }
    }
}
