using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OPCIntegrationClient
{
	public class WebResponseInfo
	{
		public string Body { get; set; }
		public string ContentEncoding { get; set; }
		public long ContentLenght { get; set; }
		public string ContentType { get; set; }
		public HttpStatusCode StatusCode { get; set; }
		public string StatusDescription { get; set; }

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine(string.Format("StatusCode {0} StatusDescription {1}", StatusCode, StatusDescription));
			sb.AppendLine(string.Format("ContentType {0} ContentEncoding {1} ContentLenght {2}", ContentType, ContentEncoding, ContentLenght));
			sb.AppendLine(string.Format("Body {0}", Body));
			return sb.ToString();
		}
	}
}
