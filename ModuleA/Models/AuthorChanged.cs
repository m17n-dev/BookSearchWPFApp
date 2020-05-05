namespace ModuleA.Models {
    public class AuthorChanged {
        public Author Author { get; private set; }

        public AuthorChanged(Author author) {
            this.Author = author;
        }
    }
}
