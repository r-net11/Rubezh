using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ResursNetwork.OSI.ApplicationLayer;
using ResursNetwork.OSI.ApplicationLayer.Devices;
using ResursNetwork.OSI.ApplicationLayer.Devices.Collections.ObjectModel;
using ResursNetwork.OSI.DataLinkLayer;
using ResursNetwork.OSI.Messages;
using ResursNetwork.OSI.Messages.Transactions;
using ResursNetwork.Management;
using ResursNetwork.Incotex.Models;
using ResursAPI.ParameterNames;

namespace ResursNetwork.Incotex.NetworkControllers.ApplicationLayer
{
    public class IncotexNetworkControllerVirtual : INetwrokController
    {
        #region Fields And Properties

        private Guid _Id = Guid.NewGuid();
        private static DeviceType[] _SuppotedDevices = 
            new DeviceType[] { DeviceType.VirtualMercury203 };
        private DevicesCollection _Devices;
        private Status _Status = Status.Stopped;
        private IDataLinkPort _Connection;
		private CancellationTokenSource _CancellationTokenSource = new CancellationTokenSource();
		private Task _NetworkPollingTask;

        public Guid Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }

        public IEnumerable<DeviceType> SuppotedDevices
        {
            get { return _SuppotedDevices; }
        }

        public DevicesCollection Devices
        {
            get { return _Devices; }
        }

        public IDataLinkPort Connection
        {
            get { throw new NotImplementedException(); }
            set 
            {                
                if (Status == Status.Running)
                {
                    throw new InvalidOperationException(
                        "Невозможно установить порт, контроллер в активном состоянии");
                }

                if (_Connection != null)
                {
                    if (_Connection.IsOpen)
                    {
                        throw new InvalidOperationException(
                            "Невозможно установить порт, порт в активном состоянии");
                    }
                    else
                    {
                        _Connection = value;
                    }
                }
                else
                {
                    _Connection = value;                    
                }
            }
        }

        public Status Status
        {
            get
            {
                return _Status;
            }
            set
            {
                if (_Status != value)
                {
                    _Status = value;

					if (_Status == Status.Running)
					{
						_NetworkPollingTask =
							Task.Factory.StartNew(NetwokPollingAction, _CancellationTokenSource.Token);
					}
					else
					{
						try
						{
							_CancellationTokenSource.Cancel();
							_NetworkPollingTask.Wait();
						}
						catch (AggregateException)
						{
							if (!_NetworkPollingTask.IsCanceled) throw;
						}
					}

					OnStatusChanged();
                }
            }
        }

        #endregion

        #region Constructors
        
        public IncotexNetworkControllerVirtual()
        {
            _Devices = new DevicesCollection(this);

			Initialization();
        }

        #endregion

        #region Methods

		public void Initialization()
		{
 
		}

        public IAsyncRequestResult Write(
            NetworkRequest request, bool isExternalCall)
        {
            throw new NotImplementedException();
        }

        public void SyncDateTime()
        {
			foreach (var device in _Devices)
			{
				device.RTC = DateTime.Now;
			}
        }

        public void Start()
        {
            Status = Status.Running;
        }

        public void Stop()
        {
            Status = Status.Stopped;
        }

		public void Dispose()
		{
			Stop();
		}

        private void OnStatusChanged()
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, new EventArgs());
            }
        }

		private void NetwokPollingAction(Object cancellationToken)
		{
			var nextPolling = DateTime.Now;
			// Симулируем работу счётчика: инкрементируем счётчики тарифов
			var cancel = (CancellationToken)cancellationToken;
			cancel.ThrowIfCancellationRequested();


			while(!cancel.IsCancellationRequested)
			{
				while (nextPolling > DateTime.Now)
				{
					Thread.Sleep(300);
				}

				foreach (var device in _Devices)
				{
					if (cancel.IsCancellationRequested)
					{
						break;
					}

					var x = (UInt32)device.Parameters[ParameterNamesMercury203.CounterTarif1].Value;
					device.Parameters[ParameterNamesMercury203.CounterTarif1].Value = x + 1;

					x = (UInt32)device.Parameters[ParameterNamesMercury203.CounterTarif2].Value;
					device.Parameters[ParameterNamesMercury203.CounterTarif2].Value = x + 1;

					x = (UInt32)device.Parameters[ParameterNamesMercury203.CounterTarif3].Value;
					device.Parameters[ParameterNamesMercury203.CounterTarif3].Value = x + 1;

					x = (UInt32)device.Parameters[ParameterNamesMercury203.CounterTarif4].Value;
					device.Parameters[ParameterNamesMercury203.CounterTarif4].Value = x + 1;
				}

				nextPolling = DateTime.Now.AddSeconds(10);
			}
		}

        #endregion

        #region Events
        
        public event EventHandler StatusChanged;
        public event EventHandler<NetworkRequestCompletedArgs> NetwrokRequestCompleted;

        #endregion
    }
}
