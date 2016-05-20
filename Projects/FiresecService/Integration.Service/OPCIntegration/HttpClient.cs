using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Common;

namespace Integration.Service.OPCIntegration
{
	internal sealed class HttpClient
	{
		private readonly HttpListener _httpListener;
		public const string IPAddress = @"http://127.0.0.1:8098/";
		public const string HttpServerAddress = @"http://127.0.0.1:8097/";
		public readonly WebResponseInfo PingSuccess;

		private string _httpServerAddress;
		private string _integrationServiceAddress;

		public HttpClient()
		{
			_httpListener = new HttpListener();
			_httpListener.Prefixes.Add(IPAddress);
			PingSuccess = new WebResponseInfo
			{
				Body = "Pong",
				StatusCode = HttpStatusCode.OK
			};
		}

		public HttpClient(string httpServerAddress, string integrationServiceAddress)
		{
			if (string.IsNullOrEmpty(httpServerAddress) || string.IsNullOrEmpty(integrationServiceAddress))
			{
				Logger.Info(httpServerAddress);
				Logger.Info(integrationServiceAddress);
				var e = new ArgumentNullException("Null argument in http client");
				Logger.Error(e);
				throw e;
			}

			_httpServerAddress = httpServerAddress;
			_integrationServiceAddress = integrationServiceAddress;
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
			try
			{
				var context = _httpListener.EndGetContext(result);
				var info = Read(context.Request);

				CreateResponse(context.Response, info.ToString());
			}
			catch (ObjectDisposedException)
			{
				//Not doing anything with the exception. HttpListener.Stop() method can throw this exception.
			}

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
			using (var reader = new StreamReader(bodyStream, Encoding.GetEncoding(1251)))//Encoding.UTF8))
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
