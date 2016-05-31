using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Common;
using Integration.Service.Entities;
using StrazhAPI.Automation.Enums;
using StrazhAPI.Enums;
using StrazhAPI.Integration.OPC;
using StrazhAPI.SKD;

namespace Integration.Service.OPCIntegration
{
	public sealed class OPCIntegrationService
	{
		private const string SetGuardCommand = "Zone:Guard:Set:{0}";
		private const string UnsetGuardCommand = "Zone:Guard:Unset:{0}";
		private const string GetConfigCommand = "Config";
		private const string PingCommand = "Ping";
		private const string ExecuteScriptCommand = "Scenario:{0}:{1}";
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
				info = SendRequestToOPCServer(PingCommand);
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
				info = SendRequestToOPCServer(GetConfigCommand);
			}
			catch (WebException e)
			{
				Logger.Info(e.ToString());
				throw;
				return new List<OPCZone>();
			}

			var xmlDoc = new XmlDocument();

			try
			{
				xmlDoc.LoadXml(info.Body);
			}
			catch (XmlException e)
			{
				Logger.Error(e);
				throw;
				return new List<OPCZone>();
			}

			const string xPath = "config/zone";
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

				var zone = new OPCZone
				{
					Name = inputZone.name,
					No = inputZone.no,
					Description = inputZone.desc,
					AutoSet = autoset != null ? Convert.ToInt32(autoset.value) : (int?)null,
					Delay = delay != null ? Convert.ToInt32(delay.value) : (int?)null,
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

			return new List<OPCZone>(opcZones.Where(x => x.Type != OPCZoneType.ASC));
		}

		public bool StopOPCIntegrationService()
		{
			if (_httpClient == null)
				return false;

			_httpClient.Stop();
			return true;
		}

		public void SetGuard(int no)
		{
			try
			{
				SendRequestToOPCServer(string.Format(SetGuardCommand, no));
			}
			catch (WebException e)
			{
				Logger.Info(e.ToString());
			}
		}

		public void UnsetGuard(int no)
		{
			try
			{
				SendRequestToOPCServer(string.Format(UnsetGuardCommand, no));
			}
			catch (WebException e)
			{
				Logger.Info(e.ToString());
			}
		}

		public List<Script> GetFiresecScripts()
		{
			XDocument xdoc;
			try
			{
				var result = SendRequestToOPCServer(GetConfigCommand);
				xdoc = XDocument.Parse(result.Body);
			}
			catch (XmlException e)
			{
				Logger.Error(e);
				return new List<Script>();
			}
			catch (WebException e)
			{
				Logger.Error(e);
				return null;
			}

			var scripts = new List<Script>();
			foreach (var node in xdoc.Descendants("scenaries").Descendants("scenario"))
			{
				var str = node.Attributes();
				var idNode = node.Attribute("id");
				var nameNode = node.Attribute("caption");
				var descriptionNode = node.Attribute("desc");
				var isEnabledNode = node.Attribute("enabled");

				scripts.Add(new Script
				{
					Id = Convert.ToInt32(idNode.Value),
					Name = nameNode != null ? nameNode.Value : null,
					Description = descriptionNode != null ? descriptionNode.Value : null,
					IsEnabled =  Convert.ToInt32(isEnabledNode.Value) != default(int)
				});
			}
			Thread.Sleep(2000);
			return scripts;
		}

		public bool ExecuteFiresecScript(Script script, FiresecCommandType type)
		{
			if (script == null) return false;

			try
			{
				SendRequestToOPCServer(string.Format(ExecuteScriptCommand, script.Id, TypeToFiresecCommand(type)));
			}
			catch (WebException e)
			{
				Logger.Info(e.ToString());
				return false;
			}

			return true;
		}

		public bool SendOPCCommandType(OPCCommandType type)
		{
			try
			{
				SendRequestToOPCServer(TypeToOPCCommand(type));
			}
			catch (WebException e)
			{
				Logger.Error(e);
				return false;
			}

			return true;
		}

		private static string TypeToFiresecCommand(FiresecCommandType type)
		{
			switch (type)
			{
				case FiresecCommandType.Run:
					return "Run";
				case FiresecCommandType.Stop:
					return "Stop";
				case FiresecCommandType.Unlock:
					return "Unblock";
				case FiresecCommandType.Lock:
					return "Block";
				default:
					return null;
			}
		}

		private static string TypeToOPCCommand(OPCCommandType type)
		{
			switch (type)
			{
				case OPCCommandType.ResetFire:
					return "ResetFire";
				case OPCCommandType.ResetAlarm:
					return "ResetAlarm";
				default:
					return null;
			}
		}

		private WebResponseInfo SendRequestToOPCServer(string message)
		{
			WebResponseInfo info;

			var webRequest = _httpClient.CreateWebRequest(message);
			using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
			{
				info = _httpClient.Read(webResponse);
			}

			return info;
		}
	}
}
