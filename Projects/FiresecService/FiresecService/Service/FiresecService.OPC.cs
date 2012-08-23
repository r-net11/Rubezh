using FiresecService.OPC;
using FiresecAPI;

namespace FiresecService.Service
{
    public partial class FiresecService
    {
        public void OPCRefresh()
        {
            if (AppSettings.IsFSEnabled)
                FiresecOPCManager.OPCRefresh();
        }

        public void OPCRegister()
        {
            FiresecOPCManager.OPCRegister();
        }

        public void OPCUnRegister()
        {
            FiresecOPCManager.OPCUnRegister();
        }
    }
}