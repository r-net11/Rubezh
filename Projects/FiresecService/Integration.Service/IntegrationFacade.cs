using Common;
using Integration.Service.OPCIntegration;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Service
{
	public sealed class IntegrationFacade : IIntegrationService
	{
		private const int DefaultTimeoutMilliseconds = 15000;
	//	#region Singleton implementation
		//private static volatile IntegrationFacade _instance;
	//	private static readonly object SyncRoot = new object();

		public IntegrationFacade()
		{
			if (!HttpListener.IsSupported)
				Logger.Error("HTTPListener is not support in that operating system.");
			else
			{
				_httpClient = new HttpClient();
				Task.Factory.StartNew(_httpClient.Start);
			}
		}

		//public static IntegrationFacade Instance
		//{
		//	get
		//	{
		//		if (_instance != null) return _instance;

		//		lock (SyncRoot)
		//		{
		//			if(_instance == null)
		//				_instance = new IntegrationFacade();
		//		}

		//		return _instance;
		//	}
		//}
		//#endregion

		private readonly HttpClient _httpClient;

		public bool PingOPCServer()
		{
			WebResponseInfo info;
			try
			{
				var webRequest = CreateWebRequest();
				using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
				{
					info = _httpClient.Read(webResponse);
				}
			}
			catch (WebException e)
			{
				Logger.Info(e.ToString());
				return false;
			}

			return info == _httpClient.PingSuccess;
		}

		private static WebRequest CreateWebRequest()
		{
			var webRequest = WebRequest.Create(HttpClient.HttpServerAddress);
			webRequest.Method = "POST";
			webRequest.ContentType = "text/xml";
			webRequest.Credentials = CredentialCache.DefaultCredentials;
			SetRequestBody(webRequest, "Ping");

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
