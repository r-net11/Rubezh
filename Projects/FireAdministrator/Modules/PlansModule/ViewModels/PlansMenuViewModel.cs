using Infrastructure.Common;

namespace PlansModule.ViewModels
{
    public class PlansMenuViewModel
    {
        public PlansMenuViewModel(OldPlansViewModel context)
        {
            Context = context;
        }

        public OldPlansViewModel Context { get; private set; }
    }
}
