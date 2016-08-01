using AutoMapper;
using Common;
using Integration.Service.Entities;
using Integration.Service.Parcers;
using StrazhAPI.Automation.Enums;
using StrazhAPI.Enums;
using StrazhAPI.Integration.OPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Integration.Service.OPCIntegration
{
	internal sealed class OPCIntegrationService
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

		public IEnumerable<Script> GetFiresecScripts()
		{
			ScriptParser parser;
			try
			{
				var result = SendRequestToOPCServer(GetConfigCommand);

				if(result == null)
					return new List<Script>();

				parser = new ScriptParser(result.Body);
			}
			catch (Exception e)
			{
				Logger.Error(e);
				throw;
			}

			return Mapper.Map<IEnumerable<ScriptMessage>, IEnumerable<Script>>(parser.GetResult());
		}

		public IEnumerable<OPCZone> GetOPCZones()
		{
			OPCZoneParser parser;
			try
			{
				var info = SendRequestToOPCServer(GetConfigCommand);

				if(info == null)
					return new List<OPCZone>();

				parser = new OPCZoneParser(info.Body);
			}
			catch (Exception e)
			{
				Logger.Error(e);
				throw;
			}
			return Mapper.Map<IEnumerable<OPCZoneMessage>, IEnumerable<OPCZone>>(parser.GetResult())
				.Where(x => x.Type != OPCZoneType.ASC)
				.ToList();
		}

		public IEnumerable<OPCZone> GetGuardZones()
		{
			return GetOPCZones().Where(x => x.Type == OPCZoneType.Guard);
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
