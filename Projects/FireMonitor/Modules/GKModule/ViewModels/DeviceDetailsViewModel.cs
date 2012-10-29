using System;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Common.GK;
using System.Collections.Generic;

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
			//UpdateAuParameters();
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

		public bool HasAUParameters
		{
			get
			{
				return Device.Driver.AUParameters.Count > 0;
			}
		}

		void UpdateAuParameters()
		{
			AUParameterValues = new List<AUParameterValue>();
			foreach (var auParameter in Device.Driver.AUParameters)
			{
				var bytes = new List<byte>();
				var databaseNo = Device.GetDatabaseNo(DatabaseType.Gk);
				bytes.Add((byte)Device.Driver.DriverTypeNo);
				bytes.Add(Device.IntAddress);
				bytes.Add((byte)(Device.ShleifNo - 1));
				bytes.Add(auParameter.No);
				var result = SendManager.Send(Device.GkDatabaseParent, 4, 128, 2, bytes);
				if (!result.HasError)
				{
					if (result.Bytes.Count > 0)
					{
						var parameterValue = BytesHelper.SubstructShort(result.Bytes, 0);
						var auParameterValue = new AUParameterValue()
						{
							Name = auParameter.Name,
							Value = parameterValue
						};
						AUParameterValues.Add(auParameterValue);
					}
				}
			}
			OnPropertyChanged("AUParameterValues");
		}

		public List<AUParameterValue> AUParameterValues { get; private set; }

		#region IWindowIdentity Members
		public Guid Guid
		{
			get { return _guid; }
		}
		#endregion
	}

	public class AUParameterValue
	{
		public string Name { get; set; }
		public int Value { get; set; }
	}
}