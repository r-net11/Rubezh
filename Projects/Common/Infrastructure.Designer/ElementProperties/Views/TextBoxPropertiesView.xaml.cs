using System.Windows.Controls;

namespace Infrastructure.Designer.ElementProperties.Views
{
	public partial class TextBoxPropertiesView : UserControl
	{
		public TextBoxPropertiesView()
		{
			InitializeComponent();
			Loaded += new System.Windows.RoutedEventHandler(TextBoxPropertiesView_Loaded);
		}

		void TextBoxPropertiesView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			_textBox.Focus();
		}
	}
}