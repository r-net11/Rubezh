using System.Windows.Controls;

namespace StrazhModule.Views
{
	public partial class DoorDetailsView : UserControl
	{
		public DoorDetailsView()
		{
			Loaded += DoorDetailsView_Loaded;
			InitializeComponent();
		}

		void DoorDetailsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			nameTextBox.Focus();
		}
	}
}