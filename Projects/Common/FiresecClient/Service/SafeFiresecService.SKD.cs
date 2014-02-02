using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecAPI.Models.Skud;
using GKProcessor;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using Infrastructure.Common;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		public void SKDSetIgnoreRegime(SKDDevice device)
		{
			SafeOperationCall(() => { FiresecService.SKDSetIgnoreRegime(device.UID); }, "SKDSetIgnoreRegime");
		}

		public void SKDResetIgnoreRegime(SKDDevice device)
		{
			SafeOperationCall(() => { FiresecService.SKDResetIgnoreRegime(device.UID); }, "SKDResetIgnoreRegime");
		}

		public void SKDOpenDevice(SKDDevice device)
		{
			SafeOperationCall(() => { FiresecService.SKDOpenDevice(device.UID); }, "SKDOpenDevice");
		}

		public void SKDCloseDevice(SKDDevice device)
		{
			SafeOperationCall(() => { FiresecService.SKDCloseDevice(device.UID); }, "SKDCloseDevice");
		}

		public void SKDExecuteDeviceCommand(SKDDevice device, XStateBit stateBit)
		{
			SafeOperationCall(() => { FiresecService.SKDExecuteDeviceCommand(device.UID, stateBit); }, "SKDExecuteDeviceCommand");
		}

		public void SKDAllowReader(SKDDevice device)
		{
			SafeOperationCall(() => { FiresecService.SKDAllowReader(device.UID); }, "SKDAllowReader");
		}

		public void SKDDenyReader(SKDDevice device)
		{
			SafeOperationCall(() => { FiresecService.SKDDenyReader(device.UID); }, "SKDDenyReader");
		}
	}
}