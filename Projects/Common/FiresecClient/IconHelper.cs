using FiresecAPI.Models;
using XFiresecAPI;

namespace FiresecClient
{
    public static class IconHelper
    {
        public static string GetUnknownDriverIcon()
        {
            return "/Controls;component/FSIcons/Unknown_Device.png";
        }
        public static string GetFSIcon(Driver driver)
        {
            return "/Controls;component/FSIcons/" + driver.DriverType.ToString() + ".png";
        }
		//public static string GetGKIcon(XDriver driver)
		//{
		//    return "/Controls;component/GKIcons/" + driver.DriverType.ToString() + ".png";
		//}
        public static string GetStateIcon(StateType stateType)
        {
            return "/Controls;component/StateIcons/" + stateType.ToString() + ".png";
        }
    }
}