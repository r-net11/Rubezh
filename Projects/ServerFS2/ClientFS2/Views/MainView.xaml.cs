using ClientFS2.ViewModels;
namespace ClientFS2.Views
{
	public partial class MainView
	{
		public MainView()
		{
			InitializeComponent();
			Loaded += new System.Windows.RoutedEventHandler(MainView_Loaded);
		}

		void MainView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			(DataContext as MainViewModel).WriteConfigurationCommand.Execute();
		}
	}
}