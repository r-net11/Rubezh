using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevicesModule.Plans.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrustructure.Plans.Elements;
using Devices = DevicesModule.ViewModels;

namespace DevicesModule.Plans.Views
{
	public partial class DevicesView : UserControl
	{
		public DevicesView()
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
			{
				this.dragStartPoint = null;
			}

			if (this.dragStartPoint.HasValue)
			{
				Devices.DeviceViewModel viewModel = (sender as Image).DataContext as Devices.DeviceViewModel;
				//if (viewModel.DesignerCanvas != null)
				//    viewModel.DesignerCanvas.Toolbox.SetDefault();
				var device = viewModel.Device;
				if (device.Driver.IsPlaceable == false)
					return;

				if (FiresecManager.LibraryConfiguration.Devices.Any(x => x.DriverId == device.DriverUID) == false)
					return;

				ElementBase plansElement = new ElementDevice()
				{
					DeviceUID = device.UID
				};

				var dataObject = new DataObject("DESIGNER_ITEM", plansElement);
				DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);
			}

			e.Handled = true;
		}
	}
}