using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace TestWebServer
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class TestContract : ITestContract
	{
		public void NewEventsAvailable(int mask)
		{
			MainWindow.AddText("NewEvent mask = " + mask.ToString());
		}
	}
}