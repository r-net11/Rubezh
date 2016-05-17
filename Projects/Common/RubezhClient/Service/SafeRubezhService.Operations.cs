using RubezhAPI;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace RubezhClient
{
	public partial class SafeRubezhService
	{
		public void Disconnect(Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.Disconnect(clientUID);
			}, "Disconnect");
		}

		public OperationResult<ServerState> GetServerState()
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetServerState(RubezhServiceFactory.UID);
			}, "GetServerState");
		}

		public PollResult Poll(Guid clientUID, int callbackIndex)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromSeconds(90));
				using (rubezhService as IDisposable)
					return rubezhService.Poll(clientUID, callbackIndex);
			}, "Poll");
		}

		public OperationResult<SecurityConfiguration> GetSecurityConfiguration()
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetSecurityConfiguration(RubezhServiceFactory.UID);
			}, "GetSecurityConfiguration");
		}

		public void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
		{
			SafeOperationCall(() =>
				{
					var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
					using (rubezhService as IDisposable)
						rubezhService.SetSecurityConfiguration(RubezhServiceFactory.UID, securityConfiguration);
				}, "SetSecurityConfiguration");
		}

		public T GetConfiguration<T>(string filename)
			where T : VersionedConfiguration, new()
		{
			var config = new T();
			return SafeOperationCall(() => config, filename);
		}

		public List<string> GetFileNamesList(string directory)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetFileNamesList(RubezhServiceFactory.UID, directory);
			}, "GetFileNamesList");
		}

		public Dictionary<string, string> GetDirectoryHash(string directory)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetDirectoryHash(RubezhServiceFactory.UID, directory);
			}, "GetDirectoryHash");
		}

		public System.IO.Stream GetServerAppDataFile(string dirAndFileName)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetServerAppDataFile(RubezhServiceFactory.UID, dirAndFileName);
			}, "GetServerAppDataFile");
		}

		public Stream GetConfig()
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetConfig(RubezhServiceFactory.UID);
			}, "GetConfig");
		}

		public OperationResult<bool> SetRemoteConfig(Stream stream)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
				{
					rubezhService.AddJournalItem(RubezhServiceFactory.UID, new RubezhAPI.Journal.JournalItem
					{
						SystemDateTime = DateTime.Now,
						JournalEventNameType = JournalEventNameType.Применение_конфигурации,
						JournalObjectType = JournalObjectType.None,
					});
					return rubezhService.SetRemoteConfig(stream);
				}
			}, "SetRemoteConfig");
		}

		public OperationResult<bool> SetLocalConfig()
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SetLocalConfig(RubezhServiceFactory.UID);
			}, "SetLocalConfig");
		}

		public string Test(string arg)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.Test(RubezhServiceFactory.UID, arg);
			}, "Test");
		}

		public string Ping()
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.Ping(RubezhServiceFactory.UID);
			}, "Ping");
		}

		public OperationResult<bool> ResetDB()
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.ResetDB(RubezhServiceFactory.UID);
			}, "ResetDB");
		}
	}
}