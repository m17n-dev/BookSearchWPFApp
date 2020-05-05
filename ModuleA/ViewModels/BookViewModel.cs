using ModuleA.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;

namespace ModuleA.ViewModels {
    public class BookViewModel {
        public Book Model { get; private set; }

        [DisplayName("Title")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(100, ErrorMessage = "Please enter {0} within {1} characters.")]
        public ReactiveProperty<string> Title { get; private set; }

        [Required(ErrorMessage = "Author is required")]
        public ReactiveProperty<Author> Author { get; private set; }

        [Required(ErrorMessage = "PublishedYear is required")]
        [RegularExpression("[0-9]+", ErrorMessage = "PublishedYear is integer")]
        [Range(1950, int.MaxValue, ErrorMessage = "PublishedYear is integer")]
        public ReactiveProperty<string> PublishedYear { get; private set; }

        [Required(ErrorMessage = "Publisher is required")]
        public ReactiveProperty<Publisher> Publisher { get; private set; }

        public ReactiveProperty<bool> HasErrors { get; private set; }

        public BookViewModel(Book model) {
            this.Model = model;

            this.Title = this.Model
                    .ToReactivePropertyAsSynchronized(
                        x => x.Title,
                        ignoreValidationErrorValue: true)
                    .SetValidateAttribute(() => this.Title);
            this.Author = this.Model
                    .ToReactivePropertyAsSynchronized(
                        x => x.Author,
                        ignoreValidationErrorValue: true)
                    .SetValidateAttribute(() => this.Author);
            this.PublishedYear = this.Model
                    .ToReactivePropertyAsSynchronized(
                        x => x.PublishedYear,
                        convert: x => x.ToString(),
                        convertBack: x => int.Parse(x),
                        ignoreValidationErrorValue: true)
                    .SetValidateAttribute(() => this.PublishedYear);
            this.Publisher = this.Model
                    .ToReactivePropertyAsSynchronized(
                        x => x.Publisher,
                        ignoreValidationErrorValue: true)
                    .SetValidateAttribute(() => this.Publisher);

            this.HasErrors = new[] {
                this.Title.ObserveHasErrors,
                this.Author.ObserveHasErrors,
                this.PublishedYear.ObserveHasErrors,
                this.Publisher.ObserveHasErrors,
            }
           .CombineLatest(x => x.Any(y => y))
           .ToReactiveProperty();
        }
    }
}
