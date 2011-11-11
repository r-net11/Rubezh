using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class UsersMenuViewModel : BaseViewModel
    {
        public UsersMenuViewModel(UsersViewModel context)
        {
            Context = context;
        }

        public UsersViewModel Context { get; private set; }
    }
}
