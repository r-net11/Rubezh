using System.Windows;
using System.Windows.Controls;

namespace SKDModule.Views
{
	public partial class LibraryView : UserControl
	{
		public LibraryView()
		{
			InitializeComponent();
		}
		void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			FrameworkElement frameworkElement = sender as FrameworkElement;
			frameworkElement.ContextMenu.DataContext = FramesTab.DataContext;
		}
	}
}