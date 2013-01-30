namespace ClientFS2
{
    public partial class ClientView
    {
        public ClientView()
        {
            InitializeComponent();
            var clientViewModel = new ClientViewModel();
            DataContext = clientViewModel;
        }
    }
}
