using FiltersModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace FiltersModule
{
    public class FilterModule
    {
        public static bool HasChanges { get; set; }
        static FiltersViewModel _filtersViewModel;

        public FilterModule()
        {
            ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(OnShowJournal);

            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        void CreateViewModels()
        {
            _filtersViewModel = new FiltersViewModel();
            _filtersViewModel.Initialize();
        }

        static void OnShowJournal(string obj)
        {
            ServiceFactory.Layout.Show(_filtersViewModel);
        }
    }
}