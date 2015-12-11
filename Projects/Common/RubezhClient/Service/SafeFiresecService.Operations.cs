using RubezhAPI;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace RubezhClient
{
	public partial class SafeFiresecService
	{
		public void Disconnect(Guid uid)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.Disconnect(uid);
			}, "Disconnect");
		}

		public OperationResult<ServerState> GetServerState()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetServerState(FiresecServiceFactory.UID);
			}, "GetServerState");
		}

		public PollResult Poll(Guid clientUID, int callbackIndex)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(90));
				using (firesecService as IDisposable)
					return firesecService.Poll(clientUID, callbackIndex);
			}, "Poll");
		}

		public OperationResult<SecurityConfiguration> GetSecurityConfiguration()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetSecurityConfiguration(FiresecServiceFactory.UID);
			}, "GetSecurityConfiguration");
		}

		public void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
		{
			SafeOperationCall(() => FiresecService.SetSecurityConfiguration(securityConfiguration), "SetSecurityConfiguration");
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
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetFileNamesList(FiresecServiceFactory.UID, directory);
			}, "GetFileNamesList");
		}

		public Dictionary<string, string> GetDirectoryHash(string directory)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetDirectoryHash(FiresecServiceFactory.UID, directory);
			}, "GetDirectoryHash");
		}

		public System.IO.Stream GetServerAppDataFile(string dirAndFileName)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetServerAppDataFile(FiresecServiceFactory.UID, dirAndFileName);
			}, "GetServerAppDataFile");
		}

		public Stream GetConfig()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetConfig(FiresecServiceFactory.UID);
			}, "GetConfig");
		}

		public void SetRemoteConfig(Stream stream)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.SetRemoteConfig(stream);
			}, "SetRemoteConfig");
		}

		public void SetLocalConfig()
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.SetLocalConfig(FiresecServiceFactory.UID);
			}, "SetLocalConfig");
		}

		public string Test(string arg)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.Test(FiresecServiceFactory.UID, arg);
			}, "Test");
		}

		public string Ping()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.Ping(FiresecServiceFactory.UID);
			}, "Ping");
		}

		public OperationResult ResetDB()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.ResetDB(FiresecServiceFactory.UID);
			}, "ResetDB");
		}
	}
}