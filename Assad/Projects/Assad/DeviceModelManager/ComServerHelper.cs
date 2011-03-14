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
            Metadata = Firesec.ComServer.GetMetaData();
        }

        public static Firesec.Metadata.config Metadata { get; set; }
    }
}
