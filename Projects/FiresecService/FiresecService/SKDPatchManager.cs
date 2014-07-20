using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using Common;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;


namespace FiresecService
{
	public static class SKDPatchManager
	{
		static string connectionString = @"Data Source=.\sqlexpress;Initial Catalog=master;Integrated Security=True"; 
		
		public static void Patch()
		{
			try
			{
				var isExists = IsExists();
				if (!isExists)
					Create();
				ApplyPatches();
			}
			catch (Exception e)
			{
				Logger.Error(e, "SKDPatchManager");
			}
		}

		static bool IsExists()
		{
			SqlConnection connection = new SqlConnection(connectionString); 
			var sqlCommand = new SqlCommand(
				@"SELECT name FROM sys.databases WHERE name = 'SKD'",
				connection);
			connection.Open();
			var reader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
			bool result = reader.Read();
			connection.Close();
			connection.Dispose();
			return result;
		}

		static void Create()
		{
			var connection = new SqlConnection(connectionString); 
			string commandText = "";
			var stream = Application.GetResourceStream(new Uri(@"pack://application:,,,/SKDDriver;component/Scripts/Create.sql"));
			using (StreamReader sr = new StreamReader(stream.Stream))
			{
				commandText = sr.ReadToEnd();
			}
			var server = new Server(new ServerConnection(connection));
			server.ConnectionContext.ExecuteNonQuery(commandText.ToString());
		}

		static void ApplyPatches()
		{
			SqlConnection connection = new SqlConnection(connectionString); 
			string commandText;
			var stream = Application.GetResourceStream(new Uri(@"pack://application:,,,/SKDDriver;component/Scripts/Patches.sql"));
			using (StreamReader sr = new StreamReader(stream.Stream))
			{
				commandText = sr.ReadToEnd();
			}
			var server = new Server(new ServerConnection(connection));
			server.ConnectionContext.ExecuteNonQuery(commandText.ToString());
		}
	}
}


