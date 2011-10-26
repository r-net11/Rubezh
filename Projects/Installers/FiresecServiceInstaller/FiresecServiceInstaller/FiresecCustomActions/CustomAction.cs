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
    }
}
