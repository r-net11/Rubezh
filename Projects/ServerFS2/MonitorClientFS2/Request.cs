using System.Collections.Generic;
namespace MonitorClientFS2
{
	public class Request
	{
		public int Id { get; set; }
		public RequestTypes RequestType { get; set; }
		public List<byte> Bytes { get; set; }
		public int Timeout { get; set; }
		public Request(int id, RequestTypes requestType)
		{
			Id = id;
			RequestType = requestType;
		}
		public Request(int id, RequestTypes requestType, List<byte> bytes, int timeout)
		{
			Id = id;
			RequestType = requestType;
			Bytes = bytes;
			Timeout = timeout;
		}
	}
}