using ModuleA.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.ComponentModel;

namespace BookSearchWpfApp.ViewModels {
    public class MainWindowViewModel : BindableBase, INotifyPropertyChanged {
        private readonly IRegionManager _regionManager;

        private string _title = "BookSearchWpfApp";
        public string Title {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DelegateCommand<string> NavigateCommand { get; private set; }

        public MainWindowViewModel(IRegionManager regionManager) {
            this._regionManager = regionManager;
            // Default View
            this._regionManager.RegisterViewWithRegion("ContentRegion", typeof(BookView));
            this.NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        private void Navigate(string navigatePath) {
            if (navigatePath != null)
                this._regionManager.RequestNavigate("ContentRegion", navigatePath);
        }
    }
}
