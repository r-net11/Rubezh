using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Designer;

namespace Infrastructure.Designer.ViewModels
{
	public partial class PlanDesignerViewModel : BaseViewModel
	{
		public double Zoom = 1;
		public double DeviceZoom { get; set;}
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