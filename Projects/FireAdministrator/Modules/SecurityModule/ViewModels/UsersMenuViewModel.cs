using Infrastructure.Common;

namespace SecurityModule.ViewModels
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