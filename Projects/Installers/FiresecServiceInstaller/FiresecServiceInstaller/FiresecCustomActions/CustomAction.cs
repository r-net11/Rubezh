using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using FiresecService;
using System.Diagnostics;

namespace FiresecCustomActions
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult JournalDataConvert(Session session)
        {
            session.Log("Begin JournalDataConvert");

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult SQLServerCheking(Session session)
        {
            session.Log("Begin Check SQL Server");
            Debugger.Break();
            var a = CheckSQLServer.CheckSQLExpress();
            if (a)
            {
                session["CHECKSQLSERVER"] = "1";
            }
            else
            {
                session["CHECKSQLSERVER"] = "0";
            }
            return ActionResult.Success;
        }
    }
}
