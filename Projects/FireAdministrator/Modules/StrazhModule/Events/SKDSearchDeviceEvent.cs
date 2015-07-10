using System;
using System.Collections.Generic;
using FiresecAPI.SKD;
using FiresecAPI.SKD.Device;
using Microsoft.Practices.Prism.Events;

namespace StrazhModule.Events
{
	public class SKDSearchDeviceEvent : CompositePresentationEvent<List<SKDDeviceSearchInfo>>
	{
	}
}