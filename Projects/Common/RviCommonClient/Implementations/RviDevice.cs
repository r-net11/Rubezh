using System;

namespace RviCommonClient
{
	public class RviDevice : IRviDevice
	{
		#region <Реализация интерфейса IRviDevice>

		public Guid Guid { get; set; }

		public string Ip { set; get; }

		public string Name { set; get; }

		public IRviChannel[] Channels { set; get; }

		#endregion </Реализация интерфейса IRviDevice>

	}
}