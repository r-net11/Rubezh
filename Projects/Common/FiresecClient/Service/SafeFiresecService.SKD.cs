using FiresecAPI;
using XFiresecAPI;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		public OperationResult<string> SKDGetDeviceInfo(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetDeviceInfo(device.UID); }, "SKDGetDeviceInfo");
		}

		public OperationResult<bool> SKDSyncronyseTime(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDSyncronyseTime(device.UID); }, "SKDSyncronyseTime");
		}

		public OperationResult<bool> SKDWriteConfiguration(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDWriteConfiguration(device.UID); }, "SKDWriteConfiguration");
		}

		public OperationResult<bool> SKDUpdateFirmware(SKDDevice device, string fileName)
		{
			return SafeOperationCall(() => { return FiresecService.SKDUpdateFirmware(device.UID, fileName); }, "SKDUpdateFirmware");
		}

		public void SKDSetRegimeOpen(SKDDevice device)
		{
			SafeOperationCall(() => { FiresecService.SKDSetRegimeOpen(device.UID); }, "SKDSetIgnoreRegime");
		}
		public void SKDSetRegimeClose(SKDDevice device)
		{
			SafeOperationCall(() => { FiresecService.SKDSetRegimeClose(device.UID); }, "SKDSetIgnoreRegime");
		}
		public void SKDSetRegimeControl(SKDDevice device)
		{
			SafeOperationCall(() => { FiresecService.SKDSetRegimeControl(device.UID); }, "SKDSetRegimeControl");
		}
		public void SKDSetRegimeConversation(SKDDevice device)
		{
			SafeOperationCall(() => { FiresecService.SKDSetRegimeConversation(device.UID); }, "SKDSetRegimeConversation");
		}

		public void SKDOpenDevice(SKDDevice device)
		{
			SafeOperationCall(() => { FiresecService.SKDOpenDevice(device.UID); }, "SKDOpenDevice");
		}

		public void SKDCloseDevice(SKDDevice device)
		{
			SafeOperationCall(() => { FiresecService.SKDCloseDevice(device.UID); }, "SKDCloseDevice");
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