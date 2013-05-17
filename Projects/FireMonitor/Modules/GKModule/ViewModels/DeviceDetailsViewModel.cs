using System;
using System.Collections.Generic;
using System.Linq;
using Common.GK;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Threading;
using System.ComponentModel;
using Infrastructure.Common.Windows;
using Infrastructure.Common;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace GKModule.ViewModels
{
	public class DeviceDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		Guid _guid;
		public XDevice Device { get; private set; }
		public XDeviceState DeviceState { get; private set; }
		DeviceControls.XDeviceControl _deviceControl;
		public DeviceCommandsViewModel DeviceCommandsViewModel { get; private set; }

		public DeviceDetailsViewModel(Guid deviceUID)
		{
			_guid = deviceUID;
			Device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			DeviceState = Device.DeviceState;
			DeviceState.StateChanged += new Action(OnStateChanged);
			DeviceCommandsViewModel = new DeviceCommandsViewModel(DeviceState);

			Title = Device.Driver.ShortName + " " + Device.DottedAddress;
			TopMost = true;
			var backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += new DoWorkEventHandler(UpdateAuParameters);
			backgroundWorker.RunWorkerAsync();
		}

		void OnStateChanged()
		{
			if (DeviceState != null && _deviceControl != null)
				_deviceControl.StateClass = DeviceState.StateClass;
			OnPropertyChanged("DeviceControlContent");
			OnPropertyChanged("DeviceState");
		}

		public object DeviceControlContent
		{
			get
			{
				var libraryDevice = XManager.XDeviceLibraryConfiguration.XDevices.FirstOrDefault(x => x.XDriverId == Device.Driver.UID);
				if (libraryDevice == null)
				{
					return null;
				}
				if (DeviceState != null)
				{
					_deviceControl = new DeviceControls.XDeviceControl()
					{
						DriverId = Device.Driver.UID,
						Width = 50,
						Height = 50,
						StateClass = DeviceState.StateClass
					};
					_deviceControl.Update();
				}

				return _deviceControl;
			}
		}

		public string PresentationZone
		{
			get { return XManager.GetPresentationZone(Device); }
		}

		public bool HasAUParameters
		{
			get { return Device.Driver.AUParameters.Count > 0; }
		}

		void UpdateAuParameters(object sender, DoWorkEventArgs e)
		{
			AUParameterValues = new ObservableCollection<AUParameterValue>();
			if (Device.KauDatabaseParent.Driver.DriverType == XDriverType.KAU)
			{
				foreach (var auParameter in Device.Driver.AUParameters)
				{
					var bytes = new List<byte>();
					var databaseNo = Device.GetDatabaseNo(DatabaseType.Kau);
					bytes.Add((byte)Device.Driver.DriverTypeNo);
					bytes.Add(Device.IntAddress);
					bytes.Add((byte)(Device.ShleifNo - 1));
					bytes.Add(auParameter.No);
					var result = SendManager.Send(Device.KauDatabaseParent, 4, 131, 2, bytes);
					if (!result.HasError)
					{
						for (int i = 0; i < 100; i++)
						{
							var no = Device.GetDatabaseNo(DatabaseType.Gk);
							result = SendManager.Send(Device.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(no));
							if (result.Bytes.Count > 0)
							{
								var resievedParameterNo = result.Bytes[63];
								if (resievedParameterNo == auParameter.No)
								{
									var parameterValue = BytesHelper.SubstructShort(result.Bytes, 64);
									Trace.WriteLine("Read parameter try " + i.ToString() + " " + auParameter.No.ToString() + " " + parameterValue.ToString());

									var auParameterValue = new AUParameterValue()
									{
										Name = auParameter.Name,
										Value = parameterValue,
										StringValue = parameterValue.ToString()
									};
									if (auParameter.Name == "Дата последнего обслуживания")
									{
										auParameterValue.StringValue = (parameterValue / 256).ToString() + "." + (parameterValue % 256).ToString();
									}
									Dispatcher.BeginInvoke(new Action(() =>
									{
										AUParameterValues.Add(auParameterValue);
									}));

									break;
								}
								Thread.Sleep(100);
							}
						}
					}
				}
			}
			else if (Device.KauDatabaseParent.Driver.DriverType == XDriverType.RSR2_KAU)
			{
				var no = Device.GetDatabaseNo(DatabaseType.Gk);
				var result = SendManager.Send(Device.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(no));
				if (!result.HasError)
				{
					if (result.Bytes.Count > 0)
					{
						Trace.WriteLine(BytesHelper.BytesToString(result.Bytes));
						for (int i = 0; i < Device.Driver.AUParameters.Count; i++)
						{
							var auParameter = Device.Driver.AUParameters[i];
							var parameterValue = BytesHelper.SubstructShort(result.Bytes, 48 + i * 2);

							var auParameterValue = new AUParameterValue()
							{
								Name = auParameter.Name,
								Value = parameterValue,
								StringValue = parameterValue.ToString()
							};
							if (auParameter.Name == "Дата последнего обслуживания")
							{
								auParameterValue.StringValue = (parameterValue / 256).ToString() + "." + (parameterValue % 256).ToString();
							}
							AUParameterValues.Add(auParameterValue);
						}
					}
				}
			}
		}

		ObservableCollection<AUParameterValue> _auParameterValues;
		public ObservableCollection<AUParameterValue> AUParameterValues
		{
			get { return _auParameterValues; }
			set
			{
				_auParameterValues = value;
				OnPropertyChanged("AUParameterValues");
			}
		}

		#region IWindowIdentity Members
		public string Guid
		{
			get { return _guid.ToString(); }
		}
		#endregion
	}

	public class AUParameterValue
	{
		public string Name { get; set; }
		public int Value { get; set; }
		public string StringValue { get; set; }
	}
}