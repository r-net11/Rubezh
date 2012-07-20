using System;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DeviceDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public XDevice Device { get; private set; }
		public XDeviceState DeviceState { get; private set; }
		private Guid _guid;

		public DeviceDetailsViewModel(Guid deviceUID)
		{
			_guid = deviceUID;
			Device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			DeviceState = XManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == deviceUID);

			Title = Device.Driver.ShortName + " " + Device.DottedAddress;
			TopMost = true;
		}


		#region IWindowIdentity Members
		public Guid Guid
		{
			get { return _guid; }
		}
		#endregion
	}
}