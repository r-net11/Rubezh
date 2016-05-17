using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiresecIntegrationClient
{
	public class WebRequestInfo
	{
		public string Body { get; set; }
		public long ContentLenght { get; set; }
		public string ContentType { get; set; }
		public string HttpMethod { get; set; }
		public Uri Url { get; set; }

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine(string.Format("HttpMethod {0}", HttpMethod));
			sb.AppendLine(string.Format("Url {0}", Url));
			sb.AppendLine(string.Format("ContentType {0}", ContentType));
			sb.AppendLine(string.Format("ContentLenght {0}", ContentLenght));
			sb.AppendLine(string.Format("Body {0}", Body));
			return sb.ToString();
		}
	}
}
