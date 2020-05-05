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
    public class BookEditViewViewModel : IInteractionRequestAware {
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        // Model 
        private readonly AppContext _model = AppContext.Instance;

        // IInteractionRequestAware 
        public Action FinishInteraction { get; set; }
        public INotification Notification { get; set; }

        public ReactiveProperty<BookViewModel> EditTarget { get; private set; }
        public ReadOnlyReactiveCollection<int> Years { get; private set; }
        public ReactiveProperty<int> YearsIndex { get; private set; }
        public ReactiveCommand CommitCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        public BookEditViewViewModel() {
            this.EditTarget = this._model.BookDetail
                .ObserveProperty(x => x.EditTarget)
                .Where(x => x != null)
                .Select(x => new BookViewModel(x))
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.Years = this._model
                .BooksMaster.Years
                .ToReadOnlyReactiveCollection(x => x);

            this.YearsIndex = this._model.BookDetail
                .ObserveProperty(x => x.YearsIndex)
                .Select(x => x)
                .ToReactiveProperty()
                .AddTo(this.Disposable);


            this.CommitCommand = this.EditTarget
                .Where(x => x != null)
                .SelectMany(x => x.HasErrors)
                .Select(x => !x)
                .ToReactiveCommand();
            this.CommitCommand.Subscribe(async _ => {
                await this._model.BookDetail.UpdateAsync();
                this.FinishInteraction();
            });

            this.CancelCommand = new DelegateCommand(CancelInteraction);
        }

        private void CancelInteraction() {
            this.FinishInteraction?.Invoke();
        }
    }
}
