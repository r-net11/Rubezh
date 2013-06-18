using System;
using System.Collections.Generic;

namespace ServerFS2
{
	public class Response
	{
		public int Id { get; set; }
		public List<byte> Bytes { get; set; }
		public TimeSpan TimeSpan { get; set; }

		public Response()
		{
		}

		public Response (string error)
		{
			SetError(error);
		}

		public bool HasError { get; set; }
		public string Error { get; set; }
		public Response SetError(string error)
		{
			HasError = true;
			Error = error;
			return this;
		}
	}
}