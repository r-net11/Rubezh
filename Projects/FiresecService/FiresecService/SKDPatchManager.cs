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
		public static void Patch()
		{
			try
			{
				var isExists = IsExists();
				if (!isExists)
					Create();
			}
			catch (Exception e)
			{
				Logger.Error(e, "SKDPatchManager");
			}
		}

		static bool IsExists()
		{
			var connection = new SqlConnection(@"Data Source=.\sqlexpress;Initial Catalog=master;Integrated Security=True");
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
			var connection = new SqlConnection(@"Data Source=.\sqlexpress;Initial Catalog=master;Integrated Security=True");
			string commandText = "";
			var createStream = Application.GetResourceStream(new Uri(@"pack://application:,,,/SKDDriver;component/Scripts/SKUD_Create.sql"));
			using (StreamReader sr = new StreamReader(createStream.Stream))
			{
				commandText = sr.ReadToEnd();
			}

			var relationsStream = Application.GetResourceStream(new Uri(@"pack://application:,,,/SKDDriver;component/Scripts/SKUD_Relations.sql"));
			using (StreamReader sr = new StreamReader(relationsStream.Stream))
			{
				commandText = commandText + " " + sr.ReadToEnd();
			}

			var server = new Server(new ServerConnection(connection));
			server.ConnectionContext.ExecuteNonQuery(commandText.ToString());
		}
	}
}


