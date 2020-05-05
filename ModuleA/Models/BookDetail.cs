using Prism.Mvvm;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ModuleA.Models {
    public class BookDetail : BindableBase {
        private readonly BooksRepository _repository = new BooksRepository();
        private ISubject<object> _interaction;
        private Book _editTarget;
        private int _yearsIndex;

        public Book EditTarget {
            get { return _editTarget; }
            private set { SetProperty(ref _editTarget, value); }
        }

        public int YearsIndex {
            get { return _yearsIndex; }
            private set { SetProperty(ref _yearsIndex, value); }
        }

        public BookDetail(ISubject<object> interaction) {
            this._interaction = interaction;
        }

        public async Task UpdateAsync() {
            await Task.Run(() => {
                this._repository.UpdateBook(this.EditTarget);
                this._interaction.OnNext(new BookChanged(this.EditTarget));
            });
        }

        public async Task SetEditTargetAsync(int id) {
            await Task.Run(() => {
                this.EditTarget = this._repository.FindBook(id);
                var results = this._repository.GetYears();
                this.YearsIndex = results.IndexOf(this.EditTarget.PublishedYear);
                //Debug.WriteLine("YearsIndex: {0}", this.YearsIndex);
            });
        }
    }
}
