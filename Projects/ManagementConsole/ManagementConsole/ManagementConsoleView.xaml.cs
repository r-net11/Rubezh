namespace ManagementConsole
{
	public partial class ManagementConsoleView
    {
		public ManagementConsoleView()
		{
			InitializeComponent();
			var managementConsoleViewModel = new ManagementConsoleViewModel();
			DataContext = managementConsoleViewModel;
		}
	}
}