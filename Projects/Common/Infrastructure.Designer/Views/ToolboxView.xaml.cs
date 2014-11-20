using System.Windows.Controls;
using System.Windows.Input;

namespace Infrastructure.Designer.Views
{
	public partial class ToolboxView : UserControl
	{
		public ToolboxView()
		{
			InitializeComponent();
		}
		private void OnListViewItemPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}
	}
}