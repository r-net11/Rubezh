using System;
using System.Linq;
using System.Threading;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using ChinaSKDDriverAPI;

namespace ChinaSKDDriver
{
	public class DeviceProcessor
	{
		public Wrapper Wrapper { get; private set; }
		public SKDDevice Device { get; private set; }
		public int LoginID { get; private set; }
		public bool IsConnected { get; private set; }
		Thread Thread;
		bool IsStopping;
		static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);
		public event Action<SKDJournalItem> NewJournalItem;

		public DeviceProcessor(SKDDevice device)
		{
			Device = device;
			Wrapper = new Wrapper();
			Wrapper.NewJournalItem -= new Action<SKDJournalItem>(Wrapper_NewJournalItem);
			Wrapper.NewJournalItem += new Action<SKDJournalItem>(Wrapper_NewJournalItem);
		}

		void Wrapper_NewJournalItem(SKDJournalItem skdJournalItem)
		{
			if (skdJournalItem.LoginID == LoginID)
			{
				if (NewJournalItem != null)
					NewJournalItem(skdJournalItem);
			}
			throw new NotImplementedException();
		}

		public void Start()
		{
			Device.State.StateClass = XStateClass.Unknown;
			foreach (var child in Device.Children)
			{
				child.State.StateClass = XStateClass.Unknown;
			}

			IsStopping = false;
			AutoResetEvent = new AutoResetEvent(false);
			Thread = new Thread(OnStart);
			Thread.Start();
		}

		public void Stop()
		{
			IsStopping = true;
			if (AutoResetEvent != null)
			{
				AutoResetEvent.Set();
				if (Thread != null)
				{
					Thread.Join(TimeSpan.FromSeconds(1));
				}
			}
		}

		void OnStart()
		{
			while (true)
			{
				try
				{
					Connect();
					if (LoginID > 0)
					{
						Thread = null;

						var callbackResult = new SKDCallbackResult();
						callbackResult.SKDStates = new SKDStates();

						Device.State.StateClass = XStateClass.Norm;
						callbackResult.SKDStates.DeviceStates.Add(Device.State);
						foreach (var child in Device.Children)
						{
							child.State.StateClass = XStateClass.Norm;
							callbackResult.SKDStates.DeviceStates.Add(child.State);
						}
						
						Processor.DoCallback(callbackResult);
						IsConnected = true;
						return;
					}

					if (IsStopping)
						return;
					if (AutoResetEvent.WaitOne(TimeSpan.FromSeconds(5)))
					{
						return;
					}
				}
				catch { }
			}
		}

		public void Reconnect()
		{
			LoginID = 0;
			Start();
		}

		public void Connect()
		{
			var addresss = "";
			var port = 0;
			var login = "";
			var password = "";

			var addressProperty = Device.Properties.FirstOrDefault(x => x.Name == "Address");
			if (addressProperty != null)
			{
				addresss = addressProperty.StringValue;
			}
			var portProperty = Device.Properties.FirstOrDefault(x => x.Name == "Port");
			if (portProperty != null)
			{
				port = portProperty.Value;
			}
			var loginProperty = Device.Properties.FirstOrDefault(x => x.Name == "Login");
			if (portProperty != null)
			{
				login = loginProperty.StringValue;
			}
			var passwordProperty = Device.Properties.FirstOrDefault(x => x.Name == "Password");
			if (passwordProperty != null)
			{
				password = passwordProperty.StringValue;
			}
			LoginID = Wrapper.Connect(addresss, port, login, password);
		}
	}
}