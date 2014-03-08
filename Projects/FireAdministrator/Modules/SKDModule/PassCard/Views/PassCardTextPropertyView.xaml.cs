using System.Windows.Controls;

namespace SKDModule.PassCard.Views
{
	public partial class PassCardTextPropertyView : UserControl
	{
		public PassCardTextPropertyView()
		{
			InitializeComponent();
			Loaded += new System.Windows.RoutedEventHandler(TextBlockPropertiesView_Loaded);
		}

		void TextBlockPropertiesView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			_textBox.Focus();
		}
	}
}