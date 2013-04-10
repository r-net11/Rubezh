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
			var textBox = nameTextBox.Template.FindName("Text", nameTextBox) as TextBox;
			if (textBox != null)
				textBox.Focus();
		}
	}
}