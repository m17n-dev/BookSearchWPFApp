using ModuleA.DataTypes.Enums;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModuleA.Models {
    public class Author : BindableBase {
        private int _id;
        private string _name;
        private DateTime _birthday;
        private GenderType _gender;
        private bool _isChecked;
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

        public DateTime Birthday {
            get { return _birthday; }
            set { SetProperty(ref _birthday, value); }
        }

        public GenderType Gender {
            get { return _gender; }
            set { SetProperty(ref _gender, value); }
        }

        public bool IsChecked {
            get { return _isChecked; }
            set { SetProperty(ref _isChecked, value); }
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
