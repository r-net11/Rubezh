using System.Collections.Generic;

namespace Common.GK
{
	public class SendResult
	{
		public List<byte> Bytes { get; set; }
		public bool HasError { get; set; }
		public string Error { get; set; }

		public SendResult(string error)
		{
			HasError = true;
			Error = error;
		}

		public SendResult(List<byte> bytes)
		{
			Bytes = bytes;
			HasError = false;
		}
	}
}