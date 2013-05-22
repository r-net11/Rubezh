namespace MonitorClientFS2
{
	public class Request
	{
		public int Id { get; set; }
		public RequestTypes RequestType { get; set; }
		public Request(int id, RequestTypes requestType)
		{
			Id = id;
			RequestType = requestType;
		}
	}
}