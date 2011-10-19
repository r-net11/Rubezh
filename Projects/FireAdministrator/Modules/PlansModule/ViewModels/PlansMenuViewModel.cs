
namespace PlansModule.ViewModels
{
    public class PlansMenuViewModel
    {
        public PlansMenuViewModel(PlansViewModel context)
        {
            Context = context;
        }

        public PlansViewModel Context { get; private set; }
    }
}
