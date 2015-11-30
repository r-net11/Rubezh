﻿using Common;
using FiresecAPI;
using FiresecService.ViewModels;
using Infrastructure.Common.BalloonTrayTip;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using SKDDriver;
using SKDDriver.Translators;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;

namespace FiresecService
{
	public static partial class PatchManager
	{
		public static string JournalConnectionString { get; private set; }

		private static string ConnectionString
		{
			get
			{
				return SKDDatabaseService.MasterConnectionString;
			}
		}

		public static void Patch()
		{
			Patch_SKD();
			Patch_DynamicDB();
		}

		private static void Patch_Journal(int no)
		{
			const string codePlace = "PatchManager.Patch_Journal";

			try
			{
				var server = new Server(new ServerConnection(new SqlConnection(ConnectionString)));
				var commandText = String.Format(@"SELECT name FROM sys.databases WHERE name = 'Journal_{0}'", no);
				var reader = server.ConnectionContext.ExecuteReader(commandText);
				var isExists = reader.Read();
				server.ConnectionContext.Disconnect();
				if (!isExists)
				{
					var createStream =
						Application.GetResourceStream(new Uri(@"pack://application:,,,/SKDDriver;component/Scripts/Journal/Create.sql"));
					using (var streamReader = new StreamReader(createStream.Stream))
					{
						commandText = streamReader.ReadToEnd();
					}
					commandText = commandText.Replace("Journal_0", String.Format("Journal_{0}", no));
					server.ConnectionContext.ExecuteNonQuery(commandText);
					server.ConnectionContext.Disconnect();
				}
				var patchesStream =
					Application.GetResourceStream(new Uri(@"pack://application:,,,/SKDDriver;component/Scripts/Journal/Patches.sql"));
				using (var streamReader = new StreamReader(patchesStream.Stream))
				{
					commandText = streamReader.ReadToEnd();
				}
				commandText = commandText.Replace("Journal_0", String.Format("Journal_{0}", no));
				server.ConnectionContext.ExecuteNonQuery(commandText);
				server.ConnectionContext.Disconnect();
			}
			catch (ConnectionFailureException e)
			{
				HandleConnectionFailureException(e, codePlace);
			}
			catch (ExecutionFailureException e)
			{
				HandleExecutionFailureException(e, codePlace);
			}
			catch (Exception e)
			{
				Logger.Error(e, codePlace);
			}
		}

		private static void Patch_PassJournal(int no)
		{
			const string codePlace = "PatchManager.Patch_PassJournal";

			try
			{
				var server = new Server(new ServerConnection(new SqlConnection(ConnectionString)));
				var commandText = String.Format(@"SELECT name FROM sys.databases WHERE name = 'PassJournal_{0}'", no);
				var reader = server.ConnectionContext.ExecuteReader(commandText);
				var isExists = reader.Read();
				server.ConnectionContext.Disconnect();
				if (!isExists)
				{
					var createStream =
						Application.GetResourceStream(new Uri(@"pack://application:,,,/SKDDriver;component/Scripts/PassJournal/Create.sql"));
					using (var streamReader = new StreamReader(createStream.Stream))
					{
						commandText = streamReader.ReadToEnd();
					}
					commandText = commandText.Replace("PassJournal_0", String.Format("PassJournal_{0}", no));
					server.ConnectionContext.ExecuteNonQuery(commandText);
					server.ConnectionContext.Disconnect();
				}
				var patchesStream =
					Application.GetResourceStream(new Uri(@"pack://application:,,,/SKDDriver;component/Scripts/PassJournal/Patches.sql"));
				using (var streamReader = new StreamReader(patchesStream.Stream))
				{
					commandText = streamReader.ReadToEnd();
				}
				commandText = commandText.Replace("PassJournal_0", String.Format("PassJournal_{0}", no));
				server.ConnectionContext.ExecuteNonQuery(commandText);
				server.ConnectionContext.Disconnect();
			}
			catch (ConnectionFailureException e)
			{
				HandleConnectionFailureException(e, codePlace);
			}
			catch (ExecutionFailureException e)
			{
				HandleExecutionFailureException(e, codePlace);
			}
			catch (Exception e)
			{
				Logger.Error(e, codePlace);
			}
		}

		private static void Patch_DynamicDB()
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
					JournalConnectionString = DBHelper.ConnectionString = SKDDatabaseService.GetConnectionString(String.Format("Journal_{0}", journalDBNo));
					JounalSynchroniser.ConnectionString = JournalConnectionString;
					JournalTranslator.ConnectionString = JournalConnectionString;
				}

				using (var skdDatabaseService = new SKDDatabaseService())
				{
					var passJournalDBNo = skdDatabaseService.MetadataTranslator.GetPassJournalNo();
					if (passJournalDBNo == 0)
						passJournalDBNo++;
					Patch_PassJournal(passJournalDBNo);
					skdDatabaseService.MetadataTranslator.AddPassJournalMetadata(passJournalDBNo, DateTime.Now, DateTime.Now);
					PassJournalTranslator.ConnectionString = SKDDatabaseService.GetConnectionString(String.Format("PassJournal_{0}", passJournalDBNo));
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

		private static void HandleConnectionFailureException(Exception e, string codePlace)
		{
			const string msg = "Не удалось подключиться к базе данных";
			UILogger.Log(String.Format("[*] {0} '{1}' ", msg, ConnectionString));
			Logger.Error(e, codePlace);
			BalloonHelper.ShowFromServer(msg);
		}

		private static void HandleExecutionFailureException(Exception e, string codePlace)
		{
			const string msg = "Возникла ошибка при работе с базой данных";
			UILogger.Log(String.Format("[*] {0}: {1}", msg, (e.InnerException == null) ? e.Message : e.InnerException.Message));
			Logger.Error(e, codePlace);
			BalloonHelper.ShowFromServer(msg);
		}
	}
}