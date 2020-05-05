using Prism.Mvvm;
using System.ComponentModel.DataAnnotations;

namespace ModuleA.Models {
    public class Book : BindableBase {
        private int _id;
        private string _title;
        private Author _author;
        private Publisher _publisher;
        private int _publishedYear;

        public int Id {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }
        [Required]
        [MaxLength(100)]
        public string Title {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        public virtual Author Author {
            get { return _author; }
            set { SetProperty(ref _author, value); }
        }
        [Required]
        [Range(1950, int.MaxValue)]
        public int PublishedYear {
            get { return _publishedYear; }
            set { SetProperty(ref _publishedYear, value); }
        }
        public virtual Publisher Publisher {
            get { return _publisher; }
            set { SetProperty(ref _publisher, value); }
        }
    }
}
