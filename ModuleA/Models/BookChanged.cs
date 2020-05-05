namespace ModuleA.Models {
    public class BookChanged {
        public Book Book { get; private set; }

        public BookChanged(Book book) {
            this.Book = book;
        }
    }
}
