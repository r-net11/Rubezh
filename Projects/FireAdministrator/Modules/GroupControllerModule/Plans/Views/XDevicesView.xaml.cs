using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FiresecAPI.Models;
using FiresecClient;
using Infrustructure.Plans.Elements;
using GKModule.Plans.ViewModels;

namespace GKModule.Plans.Views
{
	public partial class XDevicesView : UserControl
	{
		public XDevicesView()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(DevicesView_Loaded);
			_devicesDataGrid.SelectionChanged += new SelectionChangedEventHandler(DevicesView_Loaded);
		}

		void DevicesView_Loaded(object sender, RoutedEventArgs e)
		{
			if (_devicesDataGrid.SelectedItem != null)
				_devicesDataGrid.ScrollIntoView(_devicesDataGrid.SelectedItem);
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
				dragStartPoint = null;

			if (dragStartPoint.HasValue)
			{
				XDeviceViewModel viewModel = (sender as Image).DataContext as XDeviceViewModel;
				//if (viewModel.DesignerCanvas != null)
				//    viewModel.DesignerCanvas.Toolbox.SetDefault();
				var device = viewModel.Device;
				if (!device.Driver.IsDeviceOnShleif)
					return;

				if (FiresecManager.LibraryConfiguration.Devices.Any(x => x.DriverId == device.DriverUID) == false)
					return;

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