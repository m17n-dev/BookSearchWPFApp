namespace ModuleA.Models {
    public class PublisherChanged {
        public Publisher Publisher { get; private set; }

        public PublisherChanged(Publisher publisher) {
            this.Publisher = publisher;
        }
    }
}
