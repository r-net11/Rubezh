using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MuliclientAPI;

namespace FireMonitor
{
	public class MulticlientHelper
	{
		public static bool IsMulticlient = false;
		public static string MulticlientClientId;
		public static MuliclientCallback MuliclientCallback;
		public static List<MulticlientData> MulticlientDatas { get; private set; }

		public static void Start()
		{
			MuliclientCallback = new MuliclientCallback();
			MulticlientClient.Start(MuliclientCallback, MulticlientClientId);
			MulticlientClient.Muliclient.Connect(MulticlientClientId);
			MulticlientDatas = MulticlientClient.Muliclient.GetMulticlientData();
		}
	}
}