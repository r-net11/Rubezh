using GroupControllerModule.Models;
using Infrastructure.Common;

namespace GroupControllerModule.ViewModels
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