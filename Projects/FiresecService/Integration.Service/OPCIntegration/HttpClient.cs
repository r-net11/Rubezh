﻿using Infrastructure.Common;
using StrazhAPI.Integration.OPC;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Integration.Service.OPCIntegration
{
	internal sealed class HttpClient
	{
		private const int DefaultTimeoutMilliseconds = 15000;
		private HttpListener _httpListener;
		public readonly WebResponseInfo PingSuccess;

		private string _integrationAddress;
		private string _opcAddress;

		public HttpClient()
		{
			PingSuccess = new WebResponseInfo
			{
				Body = "Pong",
				StatusCode = HttpStatusCode.OK
			};
		}

		public void Start(OPCSettings settings)
		{
			if (settings == null)
				throw new ArgumentException("OPC settings is null.");

			_integrationAddress = @"http://" + ConnectionSettingsManager.RemoteAddress + ":" + settings.IntegrationPort + "/";
			_opcAddress = @"http://"
						+ (NetworkHelper.IsLocalAddress(settings.OPCAddress) ? NetworkHelper.LocalhostIp : settings.OPCAddress)
						+ ":" + settings.OPCPort + "/";

			_httpListener = new HttpListener();
			_httpListener.Prefixes.Add(_integrationAddress);

			_httpListener.Start();

			while (_httpListener.IsListening)
				ProcessRequest();
		}

		public void Stop()
		{
			if (_httpListener != null)
			{
				_httpListener.Stop();
				_httpListener.Close();
			}
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
				var listener = result.AsyncState as HttpListener;
				if (listener != null && listener.IsListening)
				{
					var context = _httpListener.EndGetContext(result);
					var info = Read(context.Request);

					CreateResponse(context.Response, info.ToString());
				}
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
				using (var reader = new StreamReader(request.InputStream, Encoding.GetEncoding(1251)))
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

			using (var reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1251)))
			{
				info.Body = reader.ReadToEnd();
			}

			return info;
		}

		private static void CreateResponse(HttpListenerResponse response, string body)
		{
			response.StatusCode = (int) HttpStatusCode.OK;
			response.StatusDescription = HttpStatusCode.OK.ToString();
			var buffer = Encoding.UTF8.GetBytes(body);
			response.ContentLength64 = buffer.Length;
			response.OutputStream.Write(buffer, 0, buffer.Length);
			response.OutputStream.Close();
		}

		public WebRequest CreateWebRequest(string body)
		{
			var webRequest = WebRequest.Create(_opcAddress);
			webRequest.Method = "POST";
			webRequest.ContentType = "text/xml";
			webRequest.Credentials = CredentialCache.DefaultCredentials;
			SetRequestBody(webRequest, body);

			return webRequest;
		}

		private static void SetRequestBody(WebRequest webRequest, string body)
		{
			var buffer = Encoding.UTF8.GetBytes(body);
			webRequest.ContentLength = buffer.Length;
			webRequest.Timeout = DefaultTimeoutMilliseconds;

			using (var requestStream = webRequest.GetRequestStream())
				requestStream.Write(buffer, 0, buffer.Length);
		}
	}
}