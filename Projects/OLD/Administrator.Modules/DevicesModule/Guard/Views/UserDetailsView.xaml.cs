using System.Windows.Controls;

namespace DevicesModule.Views
{
	public partial class UserDetailsView : UserControl
	{
		public UserDetailsView()
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