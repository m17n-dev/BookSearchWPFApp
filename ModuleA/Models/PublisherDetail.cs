using Prism.Mvvm;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ModuleA.Models {
    public class PublisherDetail : BindableBase {
        private readonly PublishersRepository _repository = new PublishersRepository();
        private ISubject<object> _interaction;
        private Publisher _editTarget;

        public Publisher EditTarget {
            get { return _editTarget; }
            private set { SetProperty(ref _editTarget, value); }
        }

        public PublisherDetail(ISubject<object> interaction) {
            this._interaction = interaction;
        }

        public async Task UpdateAsync() {
            await Task.Run(() => {
                this._repository.UpdatePublisher(this.EditTarget);
            });
            this._interaction.OnNext(new PublisherChanged(this.EditTarget));
        }

        public async Task UpdateIsCheckedAsync(bool isChecked, int id) {
            await Task.Run(() => {
                this.EditTarget = this._repository.FindPublisher(id);
            });
            await Task.Run(() => {
                this._repository.UpdateIsCheckedPublisher(this.EditTarget, isChecked);
            });
            this._interaction.OnNext(new PublisherChanged(this.EditTarget));
        }

        public async Task SetEditTargetAsync(int id) {
            await Task.Run(() => {
                this.EditTarget = this._repository.FindPublisher(id);
            });
        }
    }
}
