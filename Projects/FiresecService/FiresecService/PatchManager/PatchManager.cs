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
using SKDDriver.Translators;

namespace FiresecService
{
	public static partial class PatchManager
	{
		public static string JournalConnectionString { get; private set; }

		static string ConnectionString
		{
			get
			{
				var serverName = GlobalSettingsHelper.GlobalSettings.DBServerName;
				var connectionString = @"Data Source=.\" + serverName + ";Initial Catalog=master;Integrated Security=True;Language='English'";
				return connectionString;
			}
		}

		public static void Patch()
		{
			Patch_SKD();
			Patch_DynamicDB();
		}

		static void Patch_Journal(int no)
		{
			try
			{
				var server = new Server(new ServerConnection(new SqlConnection(ConnectionString)));
				string commandText = @"SELECT name FROM sys.databases WHERE name = 'Journal_" + no.ToString() + "'";
				var reader = server.ConnectionContext.ExecuteReader(commandText.ToString());
				bool isExists = reader.Read();
				server.ConnectionContext.Disconnect();
				if (!isExists)
				{
					var createStream = Application.GetResourceStream(new Uri(@"pack://application:,,,/SKDDriver;component/Scripts/Journal/Create.sql"));
					using (StreamReader streamReader = new StreamReader(createStream.Stream))
					{
						commandText = streamReader.ReadToEnd();
					}
					commandText = commandText.Replace("Journal_0", "Journal_" + no.ToString());
					server.ConnectionContext.ExecuteNonQuery(commandText.ToString());
					server.ConnectionContext.Disconnect();
				}
				var patchesStream = Application.GetResourceStream(new Uri(@"pack://application:,,,/SKDDriver;component/Scripts/Journal/Patches.sql"));
				using (StreamReader streamReader = new StreamReader(patchesStream.Stream))
				{
					commandText = streamReader.ReadToEnd();
				}
				commandText = commandText.Replace("Journal_0", "Journal_" + no.ToString());
				server.ConnectionContext.ExecuteNonQuery(commandText.ToString());
				server.ConnectionContext.Disconnect();
			}
			catch (ConnectionFailureException e)
			{
				UILogger.Log("Не удалось подключиться к базе данных " + ConnectionString);
				Logger.Error(e, "PatchManager.Patch_Journal");
				BalloonHelper.ShowFromServer("Не удалось подключиться к базе данных");
			}
			catch (Exception e)
			{
				Logger.Error(e, "PatchManager.Patch_Journal");
			}
		}

		static void Patch_PassJournal(int no)
		{
			try
			{
				var server = new Server(new ServerConnection(new SqlConnection(ConnectionString)));
				string commandText = @"SELECT name FROM sys.databases WHERE name = 'PassJournal_" + no.ToString() + "'";
				var reader = server.ConnectionContext.ExecuteReader(commandText.ToString());
				bool isExists = reader.Read();
				server.ConnectionContext.Disconnect();
				if (!isExists)
				{
					var createStream = Application.GetResourceStream(new Uri(@"pack://application:,,,/SKDDriver;component/Scripts/PassJournal/Create.sql"));
					using (StreamReader streamReader = new StreamReader(createStream.Stream))
					{
						commandText = streamReader.ReadToEnd();
					}
					commandText = commandText.Replace("PassJournal_0", "PassJournal_" + no.ToString());
					server.ConnectionContext.ExecuteNonQuery(commandText.ToString());
					server.ConnectionContext.Disconnect();
				}
				var patchesStream = Application.GetResourceStream(new Uri(@"pack://application:,,,/SKDDriver;component/Scripts/PassJournal/Patches.sql"));
				using (StreamReader streamReader = new StreamReader(patchesStream.Stream))
				{
					commandText = streamReader.ReadToEnd();
				}
				commandText = commandText.Replace("PassJournal_0", "PassJournal_" + no.ToString());
				server.ConnectionContext.ExecuteNonQuery(commandText.ToString());
				server.ConnectionContext.Disconnect();
			}
			catch (ConnectionFailureException e)
			{
				UILogger.Log("Не удалось подключиться к базе данных " + ConnectionString);
				Logger.Error(e, "PatchManager.Patch_PassJournal");
				BalloonHelper.ShowFromServer("Не удалось подключиться к базе данных");
			}
			catch (Exception e)
			{
				Logger.Error(e, "PatchManager.Patch_PassJournal");
			}
		}

		static void Patch_DynamicDB()
		{
			try
			{
				using (var skdDatabaseService = new SKDDatabaseService())
				{
					var journalDBNo = skdDatabaseService.MetadataTranslator.GetJournalNo();
					if (journalDBNo == 0)
						journalDBNo = 1;
					Patch_Journal(journalDBNo);
					skdDatabaseService.MetadataTranslator.AddJournalMetadata(journalDBNo, DateTime.Now, DateTime.Now);
					JournalConnectionString = DBHelper.ConnectionString = @"Data Source=.\" + GlobalSettingsHelper.GlobalSettings.DBServerName + ";Initial Catalog=Journal_" + journalDBNo.ToString() + ";Integrated Security=True;Language='English'";
					JounalSynchroniser.ConnectionString = JournalConnectionString;
					JounalTranslator.ConnectionString = JournalConnectionString;
				}

				using (var skdDatabaseService = new SKDDatabaseService())
				{
					var passJournalDBNo = skdDatabaseService.MetadataTranslator.GetPassJournalNo();
					if (passJournalDBNo == 0)
						passJournalDBNo++;
					Patch_PassJournal(passJournalDBNo);
					skdDatabaseService.MetadataTranslator.AddPassJournalMetadata(passJournalDBNo, DateTime.Now, DateTime.Now);
					PassJournalTranslator.ConnectionString = @"Data Source=.\" + GlobalSettingsHelper.GlobalSettings.DBServerName + ";Initial Catalog=PassJournal_" + passJournalDBNo.ToString() + ";Integrated Security=True;Language='English'";
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "PatchManager.Patch_NewJournal");
			}
		}

		public static OperationResult ResetDB()
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