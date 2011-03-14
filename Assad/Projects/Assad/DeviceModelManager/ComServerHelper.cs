using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeviveModelManager
{
    public static class ComServerHelper
    {
        static ComServerHelper()
        {
            Metadata = ComServer.ComServer.GetMetaData();
        }

        public static ComServer.Metadata.config Metadata { get; set; }
    }
}
