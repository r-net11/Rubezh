using Common;
using FiresecAPI;
using FiresecService.ViewModels;
using Infrastructure.Common.BalloonTrayTip;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;

namespace FiresecService
{
	public static partial class PatchManager
	{
		private static void Patch_SKD()
		{
			try
			{
				var server = new Server(new ServerConnection(new SqlConnection(ConnectionString)));
				if (!IsExistsSKD(server))
				{
					CreateSKD(server);
				}
				PatchSKDInternal(server);

				//var random = new Random();
				//for (int i = 0; i < Int32.MaxValue; i++)
				//{
				//	var journalItem = new JournalItem();
				//	journalItem.DeviceDateTime = DateTime.Now.AddMinutes(-i);
				//	journalItem.SystemDateTime = DateTime.Now.AddMinutes(-i);
				//	journalItem.JournalEventNameType = (JournalEventNameType)random.Next(100);
				//	journalItem.JournalEventDescriptionType = (JournalEventDescriptionType)random.Next(100);
				//	journalItem.ObjectUID = Guid.NewGuid();
				//	DBHelper.Add(journalItem);
				//}
			}
			catch (ConnectionFailureException e)
			{
				const string msg = "Не удалось подключиться к базе данных";
				UILogger.Log(String.Format("{0} '{1}' ", msg, ConnectionString));
				Logger.Error(e, "PatchManager.Patch_SKD");
				BalloonHelper.ShowFromServer(msg);
			}
			catch (ExecutionFailureException e)
			{
				const string msg = "Возникла ошибка при работе с базой данных";
				UILogger.Log(String.Format("{0}: {1}", msg, (e.InnerException == null) ? e.Message : e.InnerException.Message));
				Logger.Error(e, "PatchManager.Patch_SKD");
				BalloonHelper.ShowFromServer(msg);
			}
			catch (Exception e)
			{
				Logger.Error(e, "PatchManager.Patch_SKD");
			}
		}

		private static bool IsExistsSKD(Server server)
		{
			string commandText = @"SELECT name FROM sys.databases WHERE name = 'SKD'";
			var reader = server.ConnectionContext.ExecuteReader(commandText.ToString());
			bool isExists = reader.Read();
			server.ConnectionContext.Disconnect();
			return isExists;
		}

		private static void CreateSKD(Server server)
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

		private static void PatchSKDInternal(Server server)
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

		public static OperationResult Reset_SKD()
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

		private static void DropIfExistsSKD(Server server)
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
	}
}