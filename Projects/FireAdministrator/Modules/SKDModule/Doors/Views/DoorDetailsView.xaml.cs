using System.Windows.Controls;

namespace SKDModule.Views
{
	public partial class DoorDetailsView : UserControl
	{
		public DoorDetailsView()
		{
			Loaded += new System.Windows.RoutedEventHandler(DoorDetailsView_Loaded);
			InitializeComponent();
		}

		void DoorDetailsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			nameTextBox.Focus();
		}
	}
}