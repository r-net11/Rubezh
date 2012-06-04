using Infrastructure.Common;
using PlansModule.Designer;
using Infrastructure.Common.Windows.ViewModels;

namespace PlansModule.ViewModels
{
    public partial class PlanDesignerViewModel : BaseViewModel
    {
        public double Zoom = 1;
        public double DeviceZoom = 10;

        public void ChangeZoom(double zoom)
        {
            Zoom = zoom;
            DesignerCanvas.UpdateZoom();
        }

        public void ChangeDeviceZoom(double deviceZoom)
        {
            DeviceZoom = deviceZoom;
            DesignerCanvas.UpdateZoom();
        }
    }
}
