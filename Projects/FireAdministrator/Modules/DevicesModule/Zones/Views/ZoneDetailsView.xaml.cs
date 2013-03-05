using System.Windows.Controls;

namespace DevicesModule.Views
{
	public partial class ZoneDetailsView : UserControl
	{
		public ZoneDetailsView()
		{
			Loaded += new System.Windows.RoutedEventHandler(ZoneDetailsView_Loaded);
			InitializeComponent();
		}

		void ZoneDetailsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			nameTextBox.Focus();
		}
	}
}