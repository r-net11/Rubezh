using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using Common;
using FiresecAPI;
using FiresecService.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using SKDDriver;

namespace FiresecService
{
	static partial class PatchManager
	{
		static string ConnectionString
		{
			get
			{
				var serverName = GlobalSettingsHelper.GlobalSettings.DbConnectionString;
				var connectionString = @"Data Source=.\" + serverName + ";Initial Catalog=master;Integrated Security=True;Language='English'";
				return connectionString;
			}
		}

		static void Patch()
		{
			try
			{
				var server = new Server(new ServerConnection(new SqlConnection(ConnectionString)));
				if (!IsExistsSKD(server))
				{
					CreateSKD(server);
				}
				PatchSKDInternal(server);
			}
			catch (ConnectionFailureException e)
			{
				UILogger.Log("Не удалось подключиться к базе данных " + ConnectionString);
				Logger.Error(e, "PatchManager.Patch_SKD");
				BalloonHelper.ShowFromServer("Не удалось подключиться к базе данных");
			}
			catch (Exception e)
			{
				Logger.Error(e, "PatchManager.Patch_SKD");
			}
		}

		static bool IsExistsSKD(Server server)
		{
			string commandText = @"SELECT name FROM sys.databases WHERE name = 'SKD'";
			var reader = server.ConnectionContext.ExecuteReader(commandText.ToString());
			bool isExists = reader.Read();
			server.ConnectionContext.Disconnect();
			return isExists;
		}

		static void CreateSKD(Server server)
		{
			var createStream = Application.GetResourceStream(new Uri(@"pack://application:,,,/SKDDriver;component/Scripts/SKD/Create.sql"));
			string commandText;
			using (StreamReader streamReader = new StreamReader(createStream.Stream))
			{
				commandText = streamReader.ReadToEnd();
			}
			server.ConnectionContext.ExecuteNonQuery(commandText);
			server.ConnectionContext.Disconnect();
		}

		static void PatchSKDInternal(Server server)
		{
			var patchesStream = Application.GetResourceStream(new Uri(@"pack://application:,,,/SKDDriver;component/Scripts/SKD/Patches.sql"));
			string commandText;
			using (StreamReader streamReader = new StreamReader(patchesStream.Stream))
			{
				commandText = streamReader.ReadToEnd();
			}
			server.ConnectionContext.ExecuteNonQuery(commandText);
			server.ConnectionContext.Disconnect();
		}

		static OperationResult Reset_SKD()
		{
			try
			{
				var server = new Server(new ServerConnection(new SqlConnection(ConnectionString)));
				DropIfExistsSKD(server);
				CreateSKD(server);
				return new OperationResult();
			}
			catch (Exception e)
			{
				Logger.Error(e, "PatchManager.Reset_SKD");
				return new OperationResult(e.Message);
			}
		}

		static void DropIfExistsSKD(Server server)
		{
			var patchesStream = Application.GetResourceStream(new Uri(@"pack://application:,,,/SKDDriver;component/Scripts/SKD/DropIfExists.sql"));
			string commandText;
			using (StreamReader streamReader = new StreamReader(patchesStream.Stream))
			{
				commandText = streamReader.ReadToEnd();
			}
			server.ConnectionContext.ExecuteNonQuery(commandText);
			server.ConnectionContext.Disconnect();
		}

		static OperationResult ResetDB()
		{
			try
			{
				var server = new Server(new ServerConnection(new SqlConnection(ConnectionString)));
				server.Databases["SKD"].Drop();
				//string commandText = @"IF EXISTS(select * from sys.databases where name='SKD') DROP DATABASE SKD";
				//server.ConnectionContext.e.ExecuteNonQuery(commandText.ToString());
				server.ConnectionContext.Disconnect();
			}
			catch (ConnectionFailureException)
			{
				return new OperationResult("Не удалось подключиться к базе данных " + ConnectionString);
			}
			catch (Exception e)
			{
				Logger.Error(e, "PatchManager.ResetDB");
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}
	}
}