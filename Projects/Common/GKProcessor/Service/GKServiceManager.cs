using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKProcessor
{
	public static class GKServiceManager
	{
		public static GKService GKService { get; private set; }

		static GKServiceManager()
		{
			GKService = new GKService();
		}

		public static Action<string> GetUserName;
	}
}