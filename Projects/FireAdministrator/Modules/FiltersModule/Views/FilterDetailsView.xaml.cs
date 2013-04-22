using System.Windows.Controls;

namespace FiltersModule.Views
{
	public partial class FilterDetailsView : UserControl
	{
		public FilterDetailsView()
		{
			InitializeComponent();
			Loaded += new System.Windows.RoutedEventHandler(FilterDetailsView_Loaded);
		}

		void FilterDetailsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			NameTextBox.Focus();
		}
	}
}