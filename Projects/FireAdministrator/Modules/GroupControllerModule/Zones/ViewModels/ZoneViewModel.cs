using Infrastructure.Common;
using XFiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
    public class ZoneViewModel : BaseViewModel
    {
        public XZone XZone { get; set; }

        public ZoneViewModel(XZone xZone)
        {
            XZone = xZone;
        }

        public void Update()
        {
            OnPropertyChanged("XZone");
        }
    }
}