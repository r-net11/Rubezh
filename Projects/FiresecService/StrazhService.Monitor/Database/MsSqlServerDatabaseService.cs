using System;
using System.Data.SqlClient;

namespace StrazhService.Monitor.Database
{
	public class MsSqlServerDatabaseService : IDatabaseService
	{
		/// <summary>
		/// Проверяет доступность СУБД MS SQL Server
		/// </summary>
		/// <param name="ipAddress">IP-адрес сервера СУБД</param>
		/// <param name="ipPort">IP-порт сервера СУБД</param>
		/// <param name="instanceName">Название именованной установки сервера СУБД</param>
		/// <param name="useIntegratedSecurity">Метод аутентификации</param>
		/// <param name="userID">Логин (только для SQL Server аутентификации)</param>
		/// <param name="userPwd">Пароль (только для SQL Server аутентификации)</param>
		/// <param name="errors">Ошибки, возникшие в процессе проверки соединения</param>
		/// <returns>true - в случае успеха, false - в противном случае</returns>
		public bool CheckConnection(string ipAddress, int ipPort, string instanceName, bool useIntegratedSecurity, string userID, string userPwd, out string errors)
		{
			errors = null;
			var connectionString = BuildConnectionString(ipAddress, ipPort, instanceName, "master", useIntegratedSecurity, userID, userPwd);
			using (var connection = new SqlConnection(connectionString))
			{
				try
				{
					connection.Open();
				}
				catch (Exception e)
				{
					errors = e.Message;
					return false;
				}
			}
			return true;
		}

		public bool CreateBackup(string ipAddress, int ipPort, string instanceName, bool useIntegratedSecurity, string userID,
			string userPwd, string databaseName, string backupFileName, out string errors)
		{
			errors = null;
			var connectionString = BuildConnectionString(ipAddress, ipPort, instanceName, "master", useIntegratedSecurity, userID, userPwd);
			using (var connection = new SqlConnection(connectionString))
			{
				var query = string.Format("BACKUP DATABASE {0} TO DISK='{1}'", databaseName, backupFileName);
				try
				{
					using (var command = new SqlCommand(query, connection))
					{
						connection.Open();
						command.ExecuteNonQuery();
					}
				}
				catch (Exception e)
				{
					errors = e.Message;
					return false;
				}
			}
			return true;
		}

		private static string BuildConnectionString(string ipAddress, int ipPort, string instanceName, string db, bool useIntegratedSecurity, string userID, string userPwd)
		{
			var csb = new SqlConnectionStringBuilder();
			csb.DataSource = String.Format(@"{0}{1},{2}", ipAddress, String.IsNullOrEmpty(instanceName) ? String.Empty : String.Format(@"\{0}", instanceName), ipPort);
			csb.InitialCatalog = db;
			csb.IntegratedSecurity = useIntegratedSecurity;
			if (!csb.IntegratedSecurity)
			{
				csb.UserID = userID;
				csb.Password = userPwd;
			}
			return csb.ConnectionString;
		}
	}
}