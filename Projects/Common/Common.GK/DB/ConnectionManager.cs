using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.GK
{
    public static class ConnectionManager
    {
        public static GkJournalDatabase CreateGKDataContext()
        {
            return new GkJournalDatabase(@"Data Source=GkJournalDatabase.sdf;Persist Security Info=True;Max Database Size=512;File Mode='shared read'");
        }
    }
}