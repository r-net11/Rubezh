using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
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

		const int MIN_POLLING_PERIOD = 1000;

		static DeviceType[] _suppotedDevices =
			new DeviceType[] { DeviceType.VirtualMercury203 };

        Guid _id = Guid.NewGuid();
        DevicesCollection _devices;
        Status _status = Status.Stopped;
        IDataLinkPort _connection;
		CancellationTokenSource _cancellationTokenSource;
		Task _networkPollingTask;
		int _pollingPeriod;

        public Guid Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public IEnumerable<DeviceType> SuppotedDevices
        {
            get { return _suppotedDevices; }
        }

        public DevicesCollection Devices
        {
            get { return _devices; }
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

                if (_connection != null)
                {
                    if (_connection.IsOpen)
                    {
                        throw new InvalidOperationException(
                            "Невозможно установить порт, порт в активном состоянии");
                    }
                    else
                    {
                        _connection = value;
                    }
                }
                else
                {
                    _connection = value;                    
                }
            }
        }

        public Status Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (_status != value)
                {
                    _status = value;

					if (_status == Status.Running)
					{
						if (_cancellationTokenSource == null)
						{
							_cancellationTokenSource = new CancellationTokenSource();
						}
						_networkPollingTask =
							Task.Factory.StartNew(NetwokPollingAction, _cancellationTokenSource.Token);
					}
					else
					{
						try
						{
							_cancellationTokenSource.Cancel();
							_networkPollingTask.Wait();
						}
						catch (AggregateException)
						{
							if (!_networkPollingTask.IsCanceled) throw;
						}
						finally
						{
							_cancellationTokenSource.Dispose();
							_cancellationTokenSource = null;
						}
					}

					OnStatusChanged();
                }
            }
        }

		/// <summary>
        /// Период (мсек) получения данных от удалённых устройтв
        /// </summary>
		public int PollingPeriod
		{
			get { return _pollingPeriod; }
			set 
			{
				if (value > MIN_POLLING_PERIOD)
				{
					_pollingPeriod = value;
				}
				else
				{
					throw new ArgumentOutOfRangeException("DataSyncPeriod", String.Empty);
				}
			}
		}
        #endregion

        #region Constructors
        
        public IncotexNetworkControllerVirtual()
        {
            _devices = new DevicesCollection(this);
        }

        #endregion

        #region Methods

		public IAsyncRequestResult Write(
			NetworkRequest request, bool isExternalCall)
		{
			throw new NotImplementedException();
		}

        public void SyncDateTime()
        {
			foreach (var device in _devices)
			{
				device.Rtc = DateTime.Now;
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

		private void OnParameterChanged(ParameterChangedArgs args)
		{
			if (ParameterChanged != null)
			{
				ParameterChanged(this, args);
			}
		}

		private void NetwokPollingAction(Object cancellationToken)
		{
			Debug.WriteLine("Поток на обработку запущен");

			var nextPolling = DateTime.Now;
			// Симулируем работу счётчика: инкрементируем счётчики тарифов
			var cancel = (CancellationToken)cancellationToken;
			
			if (cancel.IsCancellationRequested)
			{
				return;
			}
			
			cancel.ThrowIfCancellationRequested();


			while(!cancel.IsCancellationRequested)
			{
				while (nextPolling > DateTime.Now)
				{
					Thread.Sleep(300);
				}

				foreach (var device in _devices)
				{
					if (cancel.IsCancellationRequested)
					{
						break;
					}

					if (device.Status == Management.Status.Stopped)
					{
						continue;
					}

					var x = (float)device.Parameters[ParameterNamesMercury203Virtual.CounterTarif1].Value;
					var newValue  = x + 1;
					device.Parameters[ParameterNamesMercury203Virtual.CounterTarif1].Value = newValue;
					OnParameterChanged(new ParameterChangedArgs(device.Id, ParameterNamesMercury203Virtual.CounterTarif1,
						newValue));

					if (cancel.IsCancellationRequested)
					{
						break;
					}

					x = (float)device.Parameters[ParameterNamesMercury203Virtual.CounterTarif2].Value;
					newValue = x + 1;
					device.Parameters[ParameterNamesMercury203Virtual.CounterTarif2].Value = newValue;
					OnParameterChanged(new ParameterChangedArgs(device.Id, ParameterNamesMercury203Virtual.CounterTarif2,
						newValue));

					if (cancel.IsCancellationRequested)
					{
						break;
					}

					x = (float)device.Parameters[ParameterNamesMercury203Virtual.CounterTarif3].Value;
					newValue = x + 1;
					device.Parameters[ParameterNamesMercury203Virtual.CounterTarif3].Value = newValue;
					OnParameterChanged(new ParameterChangedArgs(device.Id, ParameterNamesMercury203Virtual.CounterTarif3,
						newValue));

					if (cancel.IsCancellationRequested)
					{
						break;
					}

					x = (float)device.Parameters[ParameterNamesMercury203Virtual.CounterTarif4].Value;
					newValue = x + 1;
					device.Parameters[ParameterNamesMercury203Virtual.CounterTarif4].Value = newValue;
					OnParameterChanged(new ParameterChangedArgs(device.Id, ParameterNamesMercury203Virtual.CounterTarif3,
						newValue));
				}

				nextPolling = DateTime.Now.AddMilliseconds(_pollingPeriod);
			}

			Debug.WriteLine("Поток на обработку остановлен");
		}

        #endregion

        #region Events

		public event EventHandler StatusChanged;
		public event EventHandler<NetworkRequestCompletedArgs> NetwrokRequestCompleted;
		public event EventHandler<ParameterChangedArgs> ParameterChanged;

		#endregion


	}
}
