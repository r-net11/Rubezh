using Infrastructure.Common;

namespace FiltersModule.ViewModels
{
    public class FiltersMenuViewModel
    {
        public FiltersMenuViewModel(FiltersViewModel context)
        {
            Context = context;
        }

        public FiltersViewModel Context { get; private set; }
    }
}