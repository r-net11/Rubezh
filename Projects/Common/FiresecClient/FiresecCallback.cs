using System.ServiceModel;
using FiresecAPI;
using System.Collections.Generic;
using FiresecAPI.Models;
using System.Windows;
using System;
using Common;

namespace FiresecClient
{
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
	public class FiresecCallback : IFiresecCallback
	{
		public void Ping()
		{
		}
	}
}