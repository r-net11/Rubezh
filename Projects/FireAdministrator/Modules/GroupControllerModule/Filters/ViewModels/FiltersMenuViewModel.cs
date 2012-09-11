using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
    public class FiltersMenuViewModel : BaseViewModel
    {
        public FiltersMenuViewModel(FiltersViewModel context)
        {
            Context = context;
        }

        public FiltersViewModel Context { get; private set; }
    }
}