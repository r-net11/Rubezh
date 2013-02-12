using System.Windows.Controls;
using DiagnosticsModule.ViewModels;

namespace DiagnosticsModule.Views
{
	public partial class TreeViewTestView : UserControl
	{
		public TreeViewTestView()
		{
			InitializeComponent();
			_tree.Model = new TreeDeviceViewModel();
		}
	}
}