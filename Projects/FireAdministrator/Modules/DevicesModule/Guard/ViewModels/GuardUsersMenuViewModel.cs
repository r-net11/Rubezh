using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class GuardUsersMenuViewModel : BaseViewModel
    {
        public GuardUsersMenuViewModel(GuardUsersViewModel context)
        {
            Context = context;
        }

        public GuardUsersViewModel Context { get; private set; }
    }
}
