using ModuleA.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace ModuleA {
    public class ModuleAModule : IModule {
        public void OnInitialized(IContainerProvider containerProvider) {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterForNavigation<SearchBookView>();
            containerRegistry.RegisterForNavigation<BookView>();
            containerRegistry.RegisterForNavigation<AuthorView>();
            containerRegistry.RegisterForNavigation<PublisherView>();
        }
    }
}
