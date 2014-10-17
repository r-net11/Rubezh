using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using Common;
using FiresecService.ViewModels;
using Infrastructure.Common.BalloonTrayTip;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using FiresecAPI.Journal;

namespace FiresecService
{
	public static class PatchManager
	{
		static string ConnectionString
		{
			get { return ConfigurationManager.ConnectionStrings["SKDDriver.Properties.Settings.ConnectionString"].ConnectionString; }
		}
		
		public static void Patch()
		{
			try
			{
				var server = new Server(new ServerConnection(new SqlConnection(ConnectionString)));
				string commandText = @"SELECT name FROM sys.databases WHERE name = 'SKD'";
				var reader = server.ConnectionContext.ExecuteReader(commandText.ToString());
				bool isExists = reader.Read();
				server.ConnectionContext.Disconnect();
				if (!isExists)
				{
					var createStream = Application.GetResourceStream(new Uri(@"pack://application:,,,/SKDDriver;component/Scripts/Create.sql"));
					using (StreamReader streamReader = new StreamReader(createStream.Stream))
					{
						commandText = streamReader.ReadToEnd();
					}
					server.ConnectionContext.ExecuteNonQuery(commandText.ToString());
					server.ConnectionContext.Disconnect();
				}
				var patchesStream = Application.GetResourceStream(new Uri(@"pack://application:,,,/SKDDriver;component/Scripts/Patches.sql"));
				using (StreamReader streamReader = new StreamReader(patchesStream.Stream))
				{
					commandText = streamReader.ReadToEnd();
				}
				server.ConnectionContext.ExecuteNonQuery(commandText.ToString());
				server.ConnectionContext.Disconnect();

				//var random = new Random();
				//for (int i = 0; i < Int32.MaxValue; i++)
				//{
				//    var journalItem = new JournalItem();
				//    journalItem.DeviceDateTime = DateTime.Now.AddMinutes(-i);
				//    journalItem.SystemDateTime = DateTime.Now.AddMinutes(-i);
				//    journalItem.JournalEventNameType = (JournalEventNameType)random.Next(100);
				//    journalItem.JournalEventDescriptionType = (JournalEventDescriptionType)random.Next(100);
				//    journalItem.ObjectUID = Guid.NewGuid();
				//    FiresecService.Service.FiresecServiceManager.SafeFiresecService.AddJournalItem(journalItem);
				//}
			}
			catch (ConnectionFailureException e)
			{
				UILogger.Log("Не удалось подключиться к базе данных " + ConnectionString);
				Logger.Error(e, "PatchManager");
				BalloonHelper.ShowFromServer("Не удалось подключиться к базе данных");
			}
			catch (Exception e)
			{
				Logger.Error(e, "PatchManager");
			}
		}
	}
}