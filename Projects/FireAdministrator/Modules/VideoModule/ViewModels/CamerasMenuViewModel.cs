namespace VideoModule.ViewModels
{
    public class CamerasMenuViewModel
    {
        public CamerasMenuViewModel(CamerasViewModel context)
        {
            Context = context;
        }

        public CamerasViewModel Context { get; private set; }
    }
}