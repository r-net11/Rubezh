using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    class GuardMenuViewModel : BaseViewModel
    {
        public GuardMenuViewModel(GuardViewModel context)
        {
            Context = context;
        }

        public GuardViewModel Context { get; private set; }
    }
}
