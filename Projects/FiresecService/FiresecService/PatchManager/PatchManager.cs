﻿using Common;
using StrazhAPI;
using FiresecService.ViewModels;
using Infrastructure.Common.BalloonTrayTip;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using StrazhDAL;
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
						Application.GetResourceStream(new Uri(@"pack://application:,,,/StrazhDAL;component/Scripts/Journal/Create.sql"));
					using (var streamReader = new StreamReader(createStream.Stream))
					{
						commandText = streamReader.ReadToEnd();
					}
					commandText = commandText.Replace("Journal_0", String.Format("Journal_{0}", no));
					server.ConnectionContext.ExecuteNonQuery(commandText);
					server.ConnectionContext.Disconnect();
				}
				var patchesStream =
					Application.GetResourceStream(new Uri(@"pack://application:,,,/StrazhDAL;component/Scripts/Journal/Patches.sql"));
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
						Application.GetResourceStream(new Uri(@"pack://application:,,,/StrazhDAL;component/Scripts/PassJournal/Create.sql"));
					using (var streamReader = new StreamReader(createStream.Stream))
					{
						commandText = streamReader.ReadToEnd();
					}
					commandText = commandText.Replace("PassJournal_0", String.Format("PassJournal_{0}", no));
					server.ConnectionContext.ExecuteNonQuery(commandText);
					server.ConnectionContext.Disconnect();
				}
				var patchesStream =
					Application.GetResourceStream(new Uri(@"pack://application:,,,/StrazhDAL;component/Scripts/PassJournal/Patches.sql"));
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
                return new OperationResult(String.Format(Resources.Language.PatchManager.PatchManager.ResetDB_OperationResult, ConnectionString));
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
            UILogger.Log(String.Format(Resources.Language.PatchManager.PatchManager.HandleConnectionFailureException_Log, ConnectionString));
			Logger.Error(e, codePlace);
            BalloonHelper.ShowFromServer(Resources.Language.PatchManager.PatchManager.HandleConnectionFailureException_Log);
		}

		private static void HandleExecutionFailureException(Exception e, string codePlace)
		{
            UILogger.Log(String.Format(Resources.Language.PatchManager.PatchManager.HandleExecutionFailureException_Log, (e.InnerException == null) ? e.Message : e.InnerException.Message));
			Logger.Error(e, codePlace);
            BalloonHelper.ShowFromServer(Resources.Language.PatchManager.PatchManager.HandleExecutionFailureException_Balloon);
		}
	}
}