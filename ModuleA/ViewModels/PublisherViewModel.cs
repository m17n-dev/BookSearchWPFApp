using ModuleA.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;

namespace ModuleA.ViewModels {
    public class PublisherViewModel {
        public Publisher Model { get; private set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(100, ErrorMessage = "Please enter {0} within {1} characters.")]
        public ReactiveProperty<string> Name { get; private set; }

        [DisplayName("Address")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(100, ErrorMessage = "Please enter {0} within {1} characters.")]
        public ReactiveProperty<string> Address { get; private set; }
        public ReactiveProperty<ICollection<Book>> Books { get; private set; }
        public ReactiveProperty<bool> IsChecked { get; private set; }
        public ReactiveProperty<bool> HasErrors { get; private set; }

        public PublisherViewModel(Publisher model) {
            this.Model = model;
            this.Name = this.Model
                    .ToReactivePropertyAsSynchronized(
                        x => x.Name,
                        ignoreValidationErrorValue: true)
                    .SetValidateAttribute(() => this.Name);
            this.Address = this.Model
                    .ToReactivePropertyAsSynchronized(
                        x => x.Address,
                        ignoreValidationErrorValue: true)
                    .SetValidateAttribute(() => this.Address);
            this.Books = this.Model
                    .ToReactivePropertyAsSynchronized(
                        x => x.Books);
            this.IsChecked = new ReactiveProperty<bool>(false);
            this.HasErrors = new[] {
                this.Name.ObserveHasErrors,
                this.Address.ObserveHasErrors
            }
           .CombineLatest(x => x.Any(y => y))
           .ToReactiveProperty();
        }
    }
}
