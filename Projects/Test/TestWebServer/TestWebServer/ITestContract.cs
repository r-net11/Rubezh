using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace TestWebServer
{
	[ServiceContract]
	public interface ITestContract
	{
		[OperationContract]
		string NewEventsAvailable(string mask);
	}
}