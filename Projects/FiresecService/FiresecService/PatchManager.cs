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