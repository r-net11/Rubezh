namespace ServerFS2
{
    public partial class ServerView
    {
        public ServerView()
        {
            InitializeComponent();
            var serverViewModel = new ServerViewModel();
            DataContext = serverViewModel;
        }
    }
}
