using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceHosting
{
    public class TestComServer
    {
        public void Test()
        {
            string config = NativeComServer.GetCoreConfig();
        }
    }
}
