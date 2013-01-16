using Common.GK.DB;
using Infrastructure.Common;

namespace Common.GK
{
    public static class ConnectionManager
    {
        public static GkJournalDatabase CreateGKDataContext()
        {
			return new GkJournalDatabase(@"Data Source=" + AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf") + ";Persist Security Info=True;Max Database Size=4000");
        }
    }
}