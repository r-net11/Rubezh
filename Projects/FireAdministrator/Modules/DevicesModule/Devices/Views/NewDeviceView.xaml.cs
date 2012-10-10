using System.Windows;
using System.Windows.Controls;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.Views
{
	public partial class NewDeviceView : UserControl
	{
		public NewDeviceView()
		{
			InitializeComponent();
		}

		private void OnMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			IInputElement element = e.MouseDevice.DirectlyOver;
			if (!(element != null && element is FrameworkElement && ((FrameworkElement)element).Parent != null))
			{
				return;
			}

			var saveCancelDialogContent = DataContext as SaveCancelDialogViewModel;
			if (saveCancelDialogContent != null)
			{
				if (saveCancelDialogContent.SaveCommand.CanExecute(null))
					saveCancelDialogContent.SaveCommand.Execute();
			}
		}
	}
}