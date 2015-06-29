using System;
using Infrastructure.Common;
using SKDDriver.Translators;

namespace SKDDriver
{
	public class SKDDatabaseService : IDisposable
	{
		public DataAccess.SKDDataContext Context { get; private set; }

		public static string ConnectionString
		{
			get
			{
				var serverName = GlobalSettingsHelper.GlobalSettings.DBServerName;
				var connectionString = @"Data Source=.\" + serverName + ";Initial Catalog=SKD;Integrated Security=True;Language='English'";
				return connectionString;
			}
		}

		public SKDDatabaseService()
		{
			Context = new DataAccess.SKDDataContext(ConnectionString);
			Context.CommandTimeout = 600;

			TimeTrackTranslator = new TimeTrackTranslator(this);
			TimeTrackDocumentTranslator = new TimeTrackDocumentTranslator(this);
			TimeTrackDocumentTypeTranslator = new TimeTrackDocumentTypeTranslator(this);
		}

		public static void Tst()
		{
			EFTest.EFTest.Test();
		}
		
		public TimeTrackTranslator TimeTrackTranslator { get; private set; }
		public TimeTrackDocumentTranslator TimeTrackDocumentTranslator { get; private set; }
		public TimeTrackDocumentTypeTranslator TimeTrackDocumentTypeTranslator { get; private set; }
		public void Dispose()
		{
			Context.Dispose();
		}
	}
}