using System;
using System.Collections.Generic;
using System.Linq;
using Common.GK;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Threading;
using System.ComponentModel;

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
			BackgroundWorker bw = new BackgroundWorker();
			bw.DoWork += new DoWorkEventHandler(UpdateAuParameters);
			bw.RunWorkerAsync();
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
            AUParameterValues = new List<AUParameterValue>();
            foreach (var auParameter in Device.Driver.AUParameters)
            {
                var bytes = new List<byte>();
                var databaseNo = Device.GetDatabaseNo(DatabaseType.Kau);
                bytes.Add((byte)Device.Driver.DriverTypeNo);
                bytes.Add(Device.IntAddress);
                bytes.Add((byte)(Device.ShleifNo - 1));
                bytes.Add(auParameter.No);
                var result = SendManager.Send(Device.KauDatabaseParent, 4, 128, 2, bytes);
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
    }
}