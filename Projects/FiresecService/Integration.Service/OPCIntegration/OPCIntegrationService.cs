using Common;
using StrazhAPI.Automation.Enums;
using StrazhAPI.Enums;
using StrazhAPI.Integration.OPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

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
			return SendRequestToOPCServer(PingCommand) == _httpClient.PingSuccess;
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
			SendRequestToOPCServer(string.Format(SetGuardCommand, no));
		}

		public void UnsetGuard(int no)
		{
			SendRequestToOPCServer(string.Format(UnsetGuardCommand, no));
		}

		public List<Script> GetFiresecScripts()
		{
			Thread.Sleep(2000);
			IEnumerable<XElement> xDoc;
			try
			{
				var result = SendRequestToOPCServer(GetConfigCommand);

				if(result == null) return new List<Script>();

				xDoc = XElement.Parse(result.Body).Elements("scenaries").Elements("scenario");
			}
			catch (Exception e)
			{
				Logger.Error(e);
				throw;
			}

			return xDoc
				.Select(scriptXML =>
					new
					{
						Id = (string)scriptXML.Attribute("id"),
						Name = (string)scriptXML.Attribute("caption"),
						Description = (string)scriptXML.Attribute("desc"),
						IsEnabled = (string)scriptXML.Attribute("enabled")
					})
					.Select(script =>
					new Script
					{
						Id = script.Id != null ? Convert.ToInt32(script.Id) : default(int),
						Name = script.Name,
						Description = script.Description,
						IsEnabled = script.IsEnabled != null ? Convert.ToInt32(script.IsEnabled) != default(int) : default(bool)
					})
				.ToList();
		}

		public List<OPCZone> GetOPCZones()
		{
			IEnumerable<XElement> xDoc;
			try
			{
				var info = SendRequestToOPCServer(GetConfigCommand);

				if(info == null) return new List<OPCZone>();

				xDoc = XElement.Parse(info.Body).Elements("zone");
			}
			catch (Exception e)
			{
				Logger.Error(e);
				throw;
			}

			return xDoc
				.Select(zoneXML => new
					{
						No = (string)zoneXML.Attribute("no"),
						Name = (string)zoneXML.Attribute("name"),
						Description = (string)zoneXML.Attribute("desc"),
						Autoset = (string)zoneXML.Descendants("param").Where(x => x.Attribute("name") != null && x.Attribute("name").Value == "AutoSet").Select(x => x.Attribute("value")).FirstOrDefault(),
						Delay = (string)zoneXML.Descendants("param").Where(x => x.Attribute("name") != null && x.Attribute("name").Value == "Delay").Select(x => x.Attribute("value")).FirstOrDefault(),
						GuardZoneType = (string)zoneXML.Descendants("param").Where(x => x.Attribute("name") != null && x.Attribute("name").Value == "GuardZoneType").Select(x => x.Attribute("value")).FirstOrDefault(),
						IsSkippedTypeEnabled = (string)zoneXML.Descendants("param").Where(x => x.Attribute("name") != null && x.Attribute("name").Value == "IsSkippedTypeEnabled").Select(x => x.Attribute("value")).FirstOrDefault(),
						ZoneType = (string)zoneXML.Descendants("param").Where(x => x.Attribute("name") != null && x.Attribute("name").Value == "ZoneType").Select(x => x.Attribute("value")).FirstOrDefault()
					})
				.Select(zone => new OPCZone
					{
						No = string.IsNullOrEmpty(zone.No) ? default(int) : Convert.ToInt32(zone.No),
						Name = string.IsNullOrEmpty(zone.Name) ? null : zone.Name,
						Description = string.IsNullOrEmpty(zone.Description) ? null : zone.Description,
						AutoSet = zone.Autoset != null ? Convert.ToInt32(zone.Autoset) : (int?)null,
						Delay = zone.Delay != null ? Convert.ToInt32(zone.Delay) : (int?)null,
						GuardZoneType = zone.GuardZoneType != null ? (GuardZoneType)Enum.Parse(typeof(GuardZoneType), zone.GuardZoneType, true) : (GuardZoneType?)null,
						IsSkippedTypeEnabled = zone.IsSkippedTypeEnabled != null ? Convert.ToBoolean(zone.IsSkippedTypeEnabled) : (bool?)null,
						Type = zone.ZoneType != null ? (OPCZoneType)Enum.Parse(typeof(OPCZoneType), zone.ZoneType) : (OPCZoneType?)null
					})
				.Where(x => x.Type != OPCZoneType.ASC)
				.ToList();
		}

		public bool ExecuteFiresecScript(Script script, FiresecCommandType type)
		{
			if (script == null) return false;

			SendRequestToOPCServer(string.Format(ExecuteScriptCommand, script.Id, TypeToFiresecCommand(type)));

			return true;
		}

		public bool SendOPCCommandType(OPCCommandType type)
		{
			SendRequestToOPCServer(TypeToOPCCommand(type));
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

			try
			{
				var webRequest = _httpClient.CreateWebRequest(message);
				using (var webResponse = (HttpWebResponse) webRequest.GetResponse())
				{
					info = _httpClient.Read(webResponse);
				}
			}
			catch (WebException e)
			{
				Logger.Error(e);
				return null;
			}
			catch (Exception e)
			{
				Logger.Error(e);
				throw;
			}

			return info;
		}
	}
}
