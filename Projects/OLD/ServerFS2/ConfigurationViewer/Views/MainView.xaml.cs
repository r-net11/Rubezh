using System.Windows;
using ConfigurationViewer.DataTemplates;

namespace ConfigurationViewer
{
	public partial class MainView : Window
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