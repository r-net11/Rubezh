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