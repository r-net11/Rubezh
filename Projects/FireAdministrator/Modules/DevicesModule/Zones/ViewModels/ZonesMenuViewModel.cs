
namespace DevicesModule.ViewModels
{
    public class ZonesMenuViewModel
    {
        public ZonesMenuViewModel(ZonesViewModel context)
        {
            Context = context;
        }

        public ZonesViewModel Context { get; private set; }
    }
}