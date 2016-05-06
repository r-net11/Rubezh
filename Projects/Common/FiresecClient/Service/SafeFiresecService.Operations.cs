using System;
using System.Collections.Generic;
using System.IO;
using StrazhAPI;
using StrazhAPI.Models;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		public OperationResult<bool> Reconnect(Guid uid, string userName, string password)
		{
			return SafeOperationCall(() => { return FiresecService.Reconnect(uid, userName, password); }, "Reconnect");
		}

		public void Disconnect(Guid uid)
		{
			SafeOperationCall(() => { FiresecService.Disconnect(uid); }, "Disconnect");
		}

		public List<CallbackResult> Poll(Guid uid)
		{
			return SafeOperationCall(() => { return FiresecService.Poll(uid); }, "Poll");
		}

		public void NotifyClientsOnConfigurationChanged()
		{
			SafeOperationCall(() => { FiresecService.NotifyClientsOnConfigurationChanged(); }, "NotifyClientsOnConfigurationChanged");
		}

		public SecurityConfiguration GetSecurityConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetSecurityConfiguration(); }, "GetSecurityConfiguration");
		}

		public void CancelSKDProgress(Guid progressCallbackUID, string userName)
		{
			SafeOperationCall(() => FiresecService.CancelSKDProgress(progressCallbackUID, userName), "CancelSKDProgress");
		}

		public T GetConfiguration<T>(string filename)
			where T : VersionedConfiguration, new()
		{
			var config = new T();
			return SafeOperationCall(() => config, filename);
		}

		public List<string> GetFileNamesList(string directory)
		{
			return SafeOperationCall(() => { return FiresecService.GetFileNamesList(directory); }, "GetFileNamesList");
		}

		public Dictionary<string, string> GetDirectoryHash(string directory)
		{
			return SafeOperationCall(() => { return FiresecService.GetDirectoryHash(directory); }, "GetDirectoryHash");
		}

		public System.IO.Stream GetFile(string dirAndFileName)
		{
			return SafeOperationCall(() => { return FiresecService.GetFile(dirAndFileName); }, "GetFile");
		}

		public Stream GetConfig()
		{
			return SafeOperationCall(() => { return FiresecService.GetConfig(); }, "GetConfig");
		}

		public void SetConfig(Stream stream)
		{
			SafeOperationCall(() => { FiresecService.SetConfig(stream); }, "SetConfig");
		}

		public void SetLocalConfig()
		{
			SafeOperationCall(() => { FiresecService.SetLocalConfig(); }, "SetLocalConfig");
		}

		public string Ping()
		{
			return SafeOperationCall(() => { return FiresecService.Ping(); }, "Ping");
		}

		public List<ServerTask> GetServerTasks()
		{
			return SafeOperationCall(() => { return FiresecService.GetServerTasks(); }, "GetServerTasks");
		}

		public OperationResult ResetDB()
		{
			return SafeOperationCall(() => { return FiresecService.ResetDB(); }, "ResetDB");
		}
	}
}