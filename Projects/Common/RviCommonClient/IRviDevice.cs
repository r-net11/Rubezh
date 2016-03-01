using System;

namespace RviCommonClient
{
	public interface IRviDevice
	{
		Guid Guid { get; set; }

		string Ip { set; get; }		

		string Name { set; get; }

		IRviChannel[] Channels { set; get; }
	}
}