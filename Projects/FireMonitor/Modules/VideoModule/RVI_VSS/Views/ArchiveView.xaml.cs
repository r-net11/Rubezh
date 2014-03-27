using System.Windows;
using System.Windows.Controls;
using VideoModule.RVI_VSS.ViewModels;

namespace VideoModule.RVI_VSS.Views
{
	public partial class ArchiveView : UserControl
	{
		public ArchiveView()
		{
			InitializeComponent();
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			var arhiveViewModel = DataContext as ArchiveViewModel;
			if ((arhiveViewModel != null) && (arhiveViewModel.SelectedRecord != null))
			{
				try
				{
					PlayerWrap.InitializeCamera(arhiveViewModel.SelectedRecord);
				}
				catch { }
			}
		}
	}
}