using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using FiresecAPI.Models;
using FiresecClient;

namespace DiagnosticsModule.ViewModels
{
	//public class TreeDeviceViewModel : ITreeModel
	//{
	//    public IEnumerable GetChildren(object parent)
	//    {
	//        var children = new List<DeviceViewModel2>();
	//        var device = parent as DeviceViewModel2;
	//        if (device != null)
	//        {
	//            foreach (var child in device.Device.Children)
	//            {
	//                var deviceViewModel2 = new DeviceViewModel2(child);
	//                children.Add(deviceViewModel2);
	//            }
	//        }
	//        else
	//        {
	//            var rootDevice = FiresecManager.FiresecConfiguration.DeviceConfiguration.RootDevice;
	//            var rootDeviceViewModel2 = new DeviceViewModel2(rootDevice);
	//            children.Add(rootDeviceViewModel2);
	//        }
	//        return children;
	//    }

	//    public bool HasChild(object parent)
	//    {
	//        var device = parent as DeviceViewModel2;
	//        if (device != null)
	//        {
	//            return device.Device.Children.Count > 0;
	//        }
	//        return false;
	//    }
	//}
}