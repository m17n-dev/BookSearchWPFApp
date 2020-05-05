using ModuleA.DataTypes;
using ModuleA.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;

namespace ModuleA.ViewModels {
    public class AuthorViewModel {
        public Author Model { get; private set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(100, ErrorMessage = "Please enter {0} within {1} characters.")]
        public ReactiveProperty<string> Name { get; private set; }

        [Required(ErrorMessage = "Birthday is required")]
        public ReactiveProperty<string> Birthday { get; private set; }

        [Required(ErrorMessage = "Gender is required")]
        public ReactiveProperty<GenderType> Gender { get; private set; }

        //[Required(ErrorMessage = "Books is required")]
        public ReactiveProperty<ICollection<Book>> Books { get; private set; }

        public ReactiveProperty<bool> HasErrors { get; private set; }

        public AuthorViewModel(Author model) {
            this.Model = model;

            this.Name = this.Model
                    .ToReactivePropertyAsSynchronized(
                        x => x.Name,
                        ignoreValidationErrorValue: true)
                    .SetValidateAttribute(() => this.Name);

            this.Birthday = this.Model
                    .ToReactivePropertyAsSynchronized(
                        x => x.Birthday,
                        convert: x => x.ToString("d"),
                        convertBack: x => DateTime.Parse(x),
                        ignoreValidationErrorValue: true)
                    .SetValidateAttribute(() => this.Birthday);

            this.Gender = this.Model
                    .ToReactivePropertyAsSynchronized(
                        x => x.Gender);

            this.Books = this.Model
                    .ToReactivePropertyAsSynchronized(
                        x => x.Books);

            this.HasErrors = new[] {
                this.Name.ObserveHasErrors,
                this.Birthday.ObserveHasErrors,
                this.Gender.ObserveHasErrors,
            }
           .CombineLatest(x => x.Any(y => y))
           .ToReactiveProperty();
        }
    }
}
