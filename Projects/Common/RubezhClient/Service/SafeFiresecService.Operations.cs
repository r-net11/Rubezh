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

		public List<CallbackResult> Poll(Guid uid, int callbackIndex)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(90));
				using (firesecService as IDisposable)
					return firesecService.Poll(uid, callbackIndex);
			}, "Poll");
		}

		public SecurityConfiguration GetSecurityConfiguration()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetSecurityConfiguration(FiresecServiceFactory.UID);
			}, "GetSecurityConfiguration");
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
					return firesecService.GetFileNamesList(directory, FiresecServiceFactory.UID);
			}, "GetFileNamesList");
		}

		public Dictionary<string, string> GetDirectoryHash(string directory)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetDirectoryHash(directory, FiresecServiceFactory.UID);
			}, "GetDirectoryHash");
		}

		public System.IO.Stream GetServerAppDataFile(string dirAndFileName)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetServerAppDataFile(dirAndFileName, FiresecServiceFactory.UID);
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
					return firesecService.Test(arg, FiresecServiceFactory.UID);
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