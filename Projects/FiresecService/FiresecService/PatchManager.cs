using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using Common;
using FiresecService.ViewModels;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace FiresecService
{
	public static class PatchManager
	{
		static string connectionString
		{
			get { return global::SKDDriver.Properties.Settings.Default.ConnectionString; }
		}
		static SqlConnection connection = new SqlConnection(connectionString);
		static Server server = new Server(new ServerConnection(connection));

		public static void Patch()
		{
			try
			{
				var isExists = IsExists();
				if (!isExists)
					Create();
				ApplyPatches();
			}
			catch (ConnectionFailureException e)
			{
				UILogger.Log("Не удалось подключиться к серверу " + connectionString);
				Logger.Error(e, "SKDPatchManager");
			}
			catch (Exception e)
			{
				Logger.Error(e, "SKDPatchManager");
			}
		}

		static bool IsExists()
		{
			string commandText = @"SELECT name FROM sys.databases WHERE name = 'SKD'";
			var reader = server.ConnectionContext.ExecuteReader(commandText.ToString());
			bool result = reader.Read();
			server.ConnectionContext.Disconnect();
			return result;
		}

		static void Create()
		{
			string commandText = "";
			var stream = Application.GetResourceStream(new Uri(@"pack://application:,,,/SKDDriver;component/Scripts/Create.sql"));
			using (StreamReader sr = new StreamReader(stream.Stream))
			{
				commandText = sr.ReadToEnd();
			}
			server.ConnectionContext.ExecuteNonQuery(commandText.ToString());
			server.ConnectionContext.Disconnect();
		}

		static void ApplyPatches()
		{
			string commandText;
			var stream = Application.GetResourceStream(new Uri(@"pack://application:,,,/SKDDriver;component/Scripts/Patches.sql"));
			using (StreamReader sr = new StreamReader(stream.Stream))
			{
				commandText = sr.ReadToEnd();
			}
			server.ConnectionContext.ExecuteNonQuery(commandText.ToString());
			server.ConnectionContext.Disconnect();
		}
	}
}


