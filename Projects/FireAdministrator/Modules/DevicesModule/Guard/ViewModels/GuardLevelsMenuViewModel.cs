using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class GuardLevelsMenuViewModel : BaseViewModel
    {
        public GuardLevelsMenuViewModel(GuardLevelsViewModel context)
        {
            Context = context;
        }

        public GuardLevelsViewModel Context { get; private set; }
    }
}
