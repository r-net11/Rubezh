
namespace DevicesModule.ViewModels
{
    public class DirectionsMenuViewModel
    {
        public DirectionsMenuViewModel(DirectionsViewModel context)
        {
            Context = context;
        }

        public DirectionsViewModel Context { get; private set; }
    }
}