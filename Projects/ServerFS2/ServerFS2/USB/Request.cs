using System;
using System.Collections.Generic;

namespace ServerFS2
{
	public class Request
	{
		public DateTime StartTime = DateTime.Now;
		public int Id { get; set; }
		public List<byte> RootBytes { get; set; }
		public List<byte> Bytes { get; set; }
		public RequestTypes RequestType { get; set; }

		public Request()
		{
		}

		public Request(RequestTypes requestType)
		{
			StartTime = DateTime.Now;
			RequestType = requestType;
		}

		public Request(RequestTypes requestType, List<byte> bytes)
			: this(requestType)
		{
			Bytes = bytes;
		}
	}
}