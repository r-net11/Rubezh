using System;
using System.Collections.Generic;

namespace ServerFS2
{
	public class Request
	{
		public DateTime StartTime = DateTime.Now;
		public int Id { get; set; }
		public List<byte> Bytes { get; set; }
		public RequestTypes RequestType { get; set; }

		public Request()
		{
		}

		public Request(int id, RequestTypes requestType)
		{
			StartTime = DateTime.Now;
			Id = id;
			RequestType = requestType;
		}

		public Request(int id, RequestTypes requestType, List<byte> bytes)
			: this(id, requestType)
		{
			Bytes = bytes;
		}
	}
}