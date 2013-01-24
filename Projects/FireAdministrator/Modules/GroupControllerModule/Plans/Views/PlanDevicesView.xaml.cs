using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.ViewModels;
using Infrustructure.Plans.Elements;

namespace GKModule.Plans.Views
{
	public partial class PlanDevicesView : UserControl
	{
		public PlanDevicesView()
		{
			InitializeComponent();
		}

		private Point? dragStartPoint = null;

		private void On_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			base.OnPreviewMouseDown(e);
			this.dragStartPoint = new Point?(e.GetPosition(this));
		}

		private void On_MouseMove(object sender, MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				this.dragStartPoint = null;
			}

			if (this.dragStartPoint.HasValue)
			{
				DeviceViewModel viewModel = (sender as Image).DataContext as DeviceViewModel;
				//if (viewModel.DesignerCanvas != null)
				//    viewModel.DesignerCanvas.Toolbox.SetDefault();
				var device = viewModel.Device;
				if (device.Driver.IsPlaceable == false)
					return;

				//if (FiresecManager.LibraryConfiguration.Devices.Any(x => x.DriverId == device.DriverUID) == false)
				//    return;

				ElementBase plansElement = new ElementXDevice()
				{
					XDeviceUID = device.UID
				};

				var dataObject = new DataObject("DESIGNER_ITEM", plansElement);
				DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);
			}

			e.Handled = true;
		}
	}
}