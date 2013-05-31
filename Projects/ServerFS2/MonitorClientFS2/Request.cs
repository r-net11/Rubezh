using System;
using System.Collections.Generic;
namespace MonitorClientFS2
{
	public class Request
	{
		public int Id { get; set; }
		public RequestTypes RequestType { get; set; }
		public List<byte> Bytes { get; set; }
		public DateTime StartTime { get; private set; }
		public bool Expired { get; set; }

		public Request(int id, RequestTypes requestType)
		{
			StartTime = DateTime.Now;
			Id = id;
			RequestType = requestType;
			Expired = false;
		}

		public Request(int id, RequestTypes requestType, List<byte> bytes)
			: this(id, requestType)
		{
			Bytes = bytes;
		}

	}
}