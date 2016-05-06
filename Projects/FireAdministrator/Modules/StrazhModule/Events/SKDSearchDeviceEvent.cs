using System;
using System.Collections.Generic;
using StrazhAPI.SKD;
using StrazhAPI.SKD.Device;
using Microsoft.Practices.Prism.Events;

namespace StrazhModule.Events
{
	public class SKDSearchDeviceEvent : CompositePresentationEvent<List<SKDDeviceSearchInfo>>
	{
	}
}