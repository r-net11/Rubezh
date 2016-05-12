using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Plans.ViewModels
{
	public partial class BasePlanDesignerViewModel : BaseViewModel
	{
		public double Zoom = 1;
		public double DeviceZoom { get; set; }
		public void ChangeZoom(double zoom)
		{
			Zoom = zoom;
			DesignerCanvas.UpdateZoom();
		}
		public void ChangeDeviceZoom(double deviceZoom)
		{
			DeviceZoom = deviceZoom;
			DesignerCanvas.UpdateZoomPoint();
		}
		public void ResetZoom(double zoom, double deviceZoom)
		{
			DeviceZoom = deviceZoom;
			ChangeZoom(zoom);
		}
	}
}