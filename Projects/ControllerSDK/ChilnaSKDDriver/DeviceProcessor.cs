using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.SKD;
using System.Threading;

namespace ChinaSKDDriver
{
	public class DeviceProcessor
	{
		public Wrapper Wrapper { get; private set; }
		public SKDDevice Device { get; private set; }
		public int LoginID { get; private set; }
		Thread Thread;

		public DeviceProcessor(SKDDevice device)
		{
			Device = device;
			Wrapper = new Wrapper();
		}

		public void Run()
		{
			Thread = new Thread(OnRun);
			Thread.Start();
			Wrapper.StartWatcher();
		}

		void OnRun()
		{
			while (true)
			{
				try
				{
					Connect();
					if (LoginID > 0)
					{
						Thread = null;
						return;
					}
					Thread.Sleep(TimeSpan.FromSeconds(5));
				}
				catch { }
			}
		}

		public void Reconnect()
		{
			LoginID = 0;
			Run();
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