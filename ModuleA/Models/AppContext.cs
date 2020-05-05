using Prism.Mvvm;
using System.Reactive.Subjects;

namespace ModuleA.Models {
    public class AppContext : BindableBase {
        public static readonly AppContext Instance = new AppContext();

        private readonly Subject<object> _interaction = new Subject<object>();

        public BooksMaster BooksMaster { get; private set; }
        public BookDetail BookDetail { get; private set; }
        public AuthorsMaster AuthorsMaster { get; private set; }
        public AuthorDetail AuthorDetail { get; private set; }
        public PublishersMaster PublishersMaster { get; private set; }
        public PublisherDetail PublisherDetail { get; private set; }
        public SearchBooksMaster SearchBooksMaster { get; private set; }

        public AppContext() {
            this.BooksMaster = new BooksMaster(this._interaction);
            this.BookDetail = new BookDetail(this._interaction);
            this.AuthorsMaster = new AuthorsMaster(this._interaction);
            this.AuthorDetail = new AuthorDetail(this._interaction);
            this.PublishersMaster = new PublishersMaster(this._interaction);
            this.PublisherDetail = new PublisherDetail(this._interaction);
            this.SearchBooksMaster = new SearchBooksMaster();
        }
    }
}
