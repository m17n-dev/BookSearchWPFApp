using Prism.Mvvm;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModuleA.Models {
    public class Publisher : BindableBase {
        private int _id;
        private string _name;
        private string _address;
        private ICollection<Book> _books;

        public int Id {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }
        [Required]
        [MaxLength(100)]
        public string Name {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        [Required]
        [MaxLength(100)]
        public string Address {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }
        public virtual ICollection<Book> Books {
            get { return _books; }
            set { SetProperty(ref _books, value); }
        }

        public string NameAndId {
            get { return this.Name + " (Id: " + this.Id.ToString() + ")"; }
        }
    }
}
