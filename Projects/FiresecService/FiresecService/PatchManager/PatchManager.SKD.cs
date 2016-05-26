﻿using Common;
using StrazhAPI;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;
using System.IO;

namespace FiresecService
{
	public static partial class PatchManager
	{
		private static void Patch_SKD()
		{
			const string codePlace = "PatchManager.Patch_SKD";
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
			using (var createStream = GetResourceStream("StrazhDAL.dll", "StrazhDAL.Scripts.SKD.Create.sql"))
			{
				string commandText;
				using (var streamReader = new StreamReader(createStream))
				{
					commandText = streamReader.ReadToEnd();
				}
				server.ConnectionContext.ExecuteNonQuery(commandText);
				server.ConnectionContext.Disconnect();
			}
		}

		private static void PatchSKDInternal(Server server)
		{
			using (var patchesStream = GetResourceStream("StrazhDAL.dll", "StrazhDAL.Scripts.SKD.Patches.sql"))
			{
				string commandText;
				using (var streamReader = new StreamReader(patchesStream))
				{
					commandText = streamReader.ReadToEnd();
				}
				server.ConnectionContext.ExecuteNonQuery(commandText);
				server.ConnectionContext.Disconnect();
			}
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
			using (var patchesStream = GetResourceStream("StrazhDAL.dll", "StrazhDAL.Scripts.SKD.DropIfExists.sql"))
			{
				string commandText;
				using (var streamReader = new StreamReader(patchesStream))
				{
					commandText = streamReader.ReadToEnd();
				}
				server.ConnectionContext.ExecuteNonQuery(commandText);
				server.ConnectionContext.Disconnect();
			}
		}
	}
}