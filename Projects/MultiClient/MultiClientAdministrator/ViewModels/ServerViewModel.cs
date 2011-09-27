using MultiClient.Services;

namespace MultiClient.ViewModels
{
    public class ServerViewModel : RegionViewModel
    {
        public ServerViewModel()
        {
            TestCommand = new RelayCommand(OnTest);
            Name = "Server 1";
        }

        string _name;
        public string Name
        {
            get{return _name;}
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public RelayCommand TestCommand { get; private set; }
        void OnTest()
        {
            var serverDetailsViewModel = new ServerDetailsViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(serverDetailsViewModel);
        }
    }
}
