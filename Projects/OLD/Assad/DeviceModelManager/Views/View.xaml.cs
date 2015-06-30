using System.Windows;

namespace DeviveModelManager
{
	public partial class View : Window
	{
		public View()
		{
			InitializeComponent();
		}

		private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			TreeItem device = e.NewValue as TreeItem;
			(DataContext as ViewModel).SelectedDevice = device;
		}
	}
}