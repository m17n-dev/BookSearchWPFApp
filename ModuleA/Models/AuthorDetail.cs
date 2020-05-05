using Prism.Mvvm;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ModuleA.Models {
    public class AuthorDetail : BindableBase {
        private readonly AuthorsRepository _repository = new AuthorsRepository();
        private ISubject<object> _interaction;
        private Author _editTarget;

        public Author EditTarget {
            get { return _editTarget; }
            private set { SetProperty(ref _editTarget, value); }
        }

        public AuthorDetail(ISubject<object> interaction) {
            this._interaction = interaction;
        }

        public async Task UpdateAsync() {
            await Task.Run(() => {
                this._repository.UpdateAuthor(this.EditTarget);
                this._interaction.OnNext(new AuthorChanged(this.EditTarget));
            });
        }

        public async Task SetEditTargetAsync(int id) {
            await Task.Run(() => {
                this.EditTarget = this._repository.FindAuthor(id);
            });
        }
    }
}
