using System;
using System.Linq;
using System.Windows;
using Common;
using FiresecAPI.Models;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static void GetStates()
		{
			FiresecService.StartPing();
		}
	}
}