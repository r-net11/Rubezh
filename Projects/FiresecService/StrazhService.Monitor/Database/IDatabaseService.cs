namespace StrazhService.Monitor.Database
{
	public interface IDatabaseService
	{
		bool CheckConnection(string ipAddress, int ipPort, bool useIntegratedSecurity, string userID, string userPwd, out string errors);
		bool CreateBackup(string ipAddress, int ipPort, bool useIntegratedSecurity, string userID, string userPwd, string databaseName, string backupFileName, out string errors);
	}
}