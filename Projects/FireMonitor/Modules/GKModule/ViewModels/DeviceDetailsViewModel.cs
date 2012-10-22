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
		DeviceControls.DeviceControl _deviceControl;

		public DeviceDetailsViewModel(Guid deviceUID)
		{
			_guid = deviceUID;
			Device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			DeviceState = Device.DeviceState;
			DeviceState.StateChanged += new Action(deviceState_StateChanged);

			Title = Device.Driver.ShortName + " " + Device.DottedAddress;
			TopMost = true;
		}

		void deviceState_StateChanged()
		{
			if (DeviceState != null && _deviceControl != null)
				_deviceControl.StateType = DeviceState.StateType;
			OnPropertyChanged("DeviceControlContent");
		}

		public object DeviceControlContent
		{
			get
			{
				var libraryDevice = FiresecManager.DeviceLibraryConfiguration.Devices.FirstOrDefault(x => x.DriverId == Device.Driver.UID);
				if (libraryDevice == null)
				{
					return null;
				}
				if (DeviceState != null)
				{
					_deviceControl = new DeviceControls.DeviceControl()
					{
						DriverId = Device.Driver.UID,
						Width = 50,
						Height = 50,
						StateType = DeviceState.StateType
					};
					_deviceControl.Update();
				}

				return _deviceControl;
			}
		}

		#region IWindowIdentity Members
		public Guid Guid
		{
			get { return _guid; }
		}
		#endregion
	}
}