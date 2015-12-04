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
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.Disconnect(uid);
			}, "Disconnect");
		}

		public OperationResult<ServerState> GetServerState()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetServerState();
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
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetSecurityConfiguration();
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
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetFileNamesList(directory);
			}, "GetFileNamesList");
		}

		public Dictionary<string, string> GetDirectoryHash(string directory)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetDirectoryHash(directory);
			}, "GetDirectoryHash");
		}

		public System.IO.Stream GetServerAppDataFile(string dirAndFileName)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetServerAppDataFile(dirAndFileName);
			}, "GetServerAppDataFile");
		}

		public Stream GetConfig()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetConfig();
			}, "GetConfig");
		}

		public void SetRemoteConfig(Stream stream)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.SetRemoteConfig(stream);
			}, "SetRemoteConfig");
		}

		public void SetLocalConfig()
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.SetLocalConfig();
			}, "SetLocalConfig");
		}

		public string Test(string arg)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.Test(arg);
			}, "Test");
		}

		public string Ping()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.Ping();
			}, "Ping");
		}

		public OperationResult ResetDB()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.ResetDB();
			}, "ResetDB");
		}
	}
}