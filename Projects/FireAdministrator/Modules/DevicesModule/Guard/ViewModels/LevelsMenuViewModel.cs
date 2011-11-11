using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class LevelsMenuViewModel : BaseViewModel
    {
        public LevelsMenuViewModel(LevelsViewModel context)
        {
            Context = context;
        }

        public LevelsViewModel Context { get; private set; }
    }
}
