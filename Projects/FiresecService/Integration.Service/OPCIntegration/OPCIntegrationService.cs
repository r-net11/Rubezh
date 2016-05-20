using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Common;
using Integration.Service.Entities;
using StrazhAPI.Enums;
using StrazhAPI.Integration.OPC;
using StrazhAPI.SKD;

namespace Integration.Service.OPCIntegration
{
	public sealed class OPCIntegrationService
	{
		private const int DefaultTimeoutMilliseconds = 15000;
		private HttpClient _httpClient;

		public OPCIntegrationService()
		{
			_httpClient = new HttpClient();
		}

		/// <summary>
		/// Запускает интеграционный сервис для интеграции с ОПС.
		/// </summary>
		/// <returns>true - если запуск успешен.</returns>
		public bool StartOPCIntegrationService()
		{
			if (!HttpListener.IsSupported)
			{
				Logger.Error("HTTPListener is not support in that operating system.");
				return false;
			}

			try
			{
				//_httpClient = new HttpClient(SKDManager.SKDConfiguration.OPCSettings, SKDManager.SKDConfiguration.OPCSettings.OPCAddress);
				//Task.Factory.StartNew(_httpClient.Start);
			}
			catch (Exception e)
			{
				Logger.Error(e);
				throw;
			}

			return true;
		}

		/// <summary>
		/// Отправляет команду Ping ОПС серверу и получает ответ.
		/// </summary>
		/// <returns>true - если ответ получен.</returns>
		public bool PingOPCServer()
		{
			WebResponseInfo info;
			try
			{
				var webRequest = CreateWebRequest("Ping");
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

		private static WebRequest CreateWebRequest(string body)
		{
			var webRequest = WebRequest.Create(HttpClient.HttpServerAddress);
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

		public List<OPCZone> GetOPCZones()
		{
			WebResponseInfo info;
			try
			{
				var webRequest = CreateWebRequest("Config");
				using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
				{
					info = _httpClient.Read(webResponse);
				}
			}
			catch (WebException e)
			{
				Logger.Info(e.ToString());
				return new List<OPCZone>();
			}

			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(info.Body);

			var xPath = "config/zone";
			var nodes = xmlDoc.SelectNodes(xPath);

			var inputZones = new List<zone>();

			foreach (XmlNode node in nodes)
			{
				var str = node.OuterXml;
				using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(str)))
				{
					var serializer = new XmlSerializer(typeof(zone));
					inputZones.Add(serializer.Deserialize(stream) as zone);
				}
			}

			var opcZones = new List<OPCZone>();
			foreach (var inputZone in inputZones)
			{
				var autoset = inputZone.param.FirstOrDefault(x => x.name == "AutoSet");
				var delay = inputZone.param.FirstOrDefault(x => x.name == "Delay");
				var guardZoneType = inputZone.param.FirstOrDefault(x => x.name == "GuardZoneType");
				var isSkippedTypeEnabled = inputZone.param.FirstOrDefault(x => x.name == "IsSkippedTypeEnabled");
				var zoneType = inputZone.param.FirstOrDefault(x => x.name == "ZoneType");
				//GuardZoneType = guardZoneType != null ? (GuardZoneType?) Enum.Parse(typeof (GuardZoneType?), guardZoneType.value) : null,

				var zone = new OPCZone
				{
					Name = inputZone.name,
					No = inputZone.no,
					Description = inputZone.desc,
					AutoSet = autoset != null ? Convert.ToInt32(autoset.value) : (int?)null,
					Delay = delay != null ? Convert.ToInt32(delay.value) : (int?)null,
					//	GuardZoneType = guardZoneType != null ? (GuardZoneType?) Enum.Parse(typeof (GuardZoneType?), guardZoneType.value) : null,
					IsSkippedTypeEnabled = isSkippedTypeEnabled != null ? Convert.ToBoolean(isSkippedTypeEnabled) : (bool?)null
				};

				if (zoneType != null)
				{
					OPCZoneType zoneTypeResult;
					if (Enum.TryParse(zoneType.value, true, out zoneTypeResult))
					{
						zone.Type = zoneTypeResult;
					}
				}

				if (guardZoneType != null)
				{
					GuardZoneType guardZoneTypeResult;
					if (Enum.TryParse(guardZoneType.value, true, out guardZoneTypeResult))
					{
						zone.GuardZoneType = guardZoneTypeResult;
					}
				}

				opcZones.Add(zone);
			}

			return new List<OPCZone>(opcZones);
		}
	}
}
