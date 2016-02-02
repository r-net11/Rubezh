using GKModule.ViewModels;
using System.Windows.Controls;

namespace GKModule.Views
{
	public partial class DeviceDetailsView : UserControl
	{
		public DeviceDetailsView()
		{
			InitializeComponent();
			SizeChanged += ToolTipVisibleChange;
			logicTextBlock.SizeChanged += ToolTipVisibleChange;
		}

		private void ToolTipVisibleChange(object sender, System.Windows.SizeChangedEventArgs e)
		{
			var viewModel = DataContext as DeviceDetailsViewModel;
			if (viewModel != null)
			{
				viewModel.IsLogicToolTipVisible = logicTextBlock.ActualWidth > rightColumn.ActualWidth;
			}
		}
	}
}