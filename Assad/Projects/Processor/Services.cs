using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Processor
{
    public static class Services
    {
        static Services()
        {
            LogEngine = new Logger.LogEngine();
            NetManager = new NetManager();
        }

        public static Logger.LogEngine LogEngine { get; private set; }
        public static NetManager NetManager { get; private set; }
    }
}
