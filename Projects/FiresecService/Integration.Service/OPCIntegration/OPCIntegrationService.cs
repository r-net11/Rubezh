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

		private HttpClient _httpClient;

		public OPCIntegrationService()
		{
			if (HttpListener.IsSupported) return;

			Logger.Error("Http listener is not support on this platform.");
			throw new NotSupportedException("Http listener");
		}

		/// <summary>
		/// Запускает интеграционный сервис для интеграции с ОПС.
		/// </summary>
		/// <returns>true - если запуск успешен.</returns>
		public bool StartOPCIntegrationService(OPCSettings settings)
		{
			try
			{
				_httpClient = new HttpClient();
				Task.Factory.StartNew(() => _httpClient.Start(settings));
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
				var webRequest = _httpClient.CreateWebRequest("Ping");
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


		public List<OPCZone> GetOPCZones()
		{
			WebResponseInfo info;
			try
			{
				var webRequest = _httpClient.CreateWebRequest("Config");
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

			xmlDoc.LoadXml(info.Body); //TODO: Handle XmlException

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

		public bool StopOPCIntegrationService()
		{
			if (_httpClient == null)
				return false;

			_httpClient.Stop();
			return true;
		}
	}
}
