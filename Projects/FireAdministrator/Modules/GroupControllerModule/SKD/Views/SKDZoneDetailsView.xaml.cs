using System.Windows.Controls;

namespace GKModule.Views
{
	public partial class SKDZoneDetailsView : UserControl
	{
		public SKDZoneDetailsView()
		{
			Loaded += new System.Windows.RoutedEventHandler(SKDZoneDetailsView_Loaded);
			InitializeComponent();
		}

		void SKDZoneDetailsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			nameTextBox.Focus();
		}
	}
}