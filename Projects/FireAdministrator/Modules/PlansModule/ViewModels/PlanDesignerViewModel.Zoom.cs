using Infrastructure.Common;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
    public partial class PlanDesignerViewModel : BaseViewModel
    {
        public double Zoom = 1;

        public void ChangeZoom(double zoom)
        {
            Zoom = zoom;
            DesignerCanvas.Zoom(zoom);
        }
    }
}
