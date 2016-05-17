using System;
using System.IO;
using System.Net;
using System.Text;
using Common;

namespace FiresecIntegrationClient
{
	public class HttpClient
	{
		private readonly HttpListener _httpListener;
		public const string IPAddress = @"http://127.0.0.1:8098/";

		public HttpClient()
		{
			//if (!HttpListener.IsSupported)
			//{
			//	Logger.Error("HTTPListener is not support in that operating system.");
			//	throw new PlatformNotSupportedException("HTTPListener");
			//}
			_httpListener = new HttpListener();
			_httpListener.Prefixes.Add(IPAddress);
		}

		public void Start()
		{
			_httpListener.Start();

			while (_httpListener.IsListening)
				ProcessRequest();
		}

		public void Stop()
		{
			_httpListener.Stop();
		}

		public void ProcessRequest()
		{
			var result = _httpListener.BeginGetContext(ListenerCallback, _httpListener);
			result.AsyncWaitHandle.WaitOne();
		}

		public void ListenerCallback(IAsyncResult result)
		{
			var context = _httpListener.EndGetContext(result);
			var info = Read(context.Request);

			CreateResponse(context.Response, info.ToString());
		}

		public WebRequestInfo Read(HttpListenerRequest request)
		{
			var info = new WebRequestInfo {HttpMethod = request.HttpMethod, Url = request.Url};

			if (request.HasEntityBody)
			{
				using(var bodyStream = request.InputStream)
				using (var reader = new StreamReader(bodyStream, Encoding.GetEncoding(1251)))
				{
					if (request.ContentType != null)
						info.ContentType = request.ContentType;

					info.ContentLenght = request.ContentLength64;
					info.Body = reader.ReadToEnd();
				}
			}

			return info;
		}

		public WebResponseInfo Read(HttpWebResponse response)
		{
			var info = new WebResponseInfo
			{
				StatusCode = response.StatusCode,
				StatusDescription = response.StatusDescription,
				ContentEncoding = response.ContentEncoding,
				ContentLenght = response.ContentLength,
				ContentType = response.ContentType
			};

			using(var bodyStream = response.GetResponseStream())
			using (var reader = new StreamReader(bodyStream, Encoding.UTF8))
			{
				info.Body = reader.ReadToEnd();
			}

			return info;
		}

		private void CreateResponse(HttpListenerResponse response, string body)
		{
			response.StatusCode = (int) HttpStatusCode.OK;
			response.StatusDescription = HttpStatusCode.OK.ToString();
			var buffer = Encoding.UTF8.GetBytes(body);
			response.ContentLength64 = buffer.Length;
			response.OutputStream.Write(buffer, 0, buffer.Length);
			response.OutputStream.Close();
		}
	}
}
