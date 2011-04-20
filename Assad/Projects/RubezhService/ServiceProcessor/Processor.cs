using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using ServiceApi;

namespace ServiseProcessor
{
    public class Processor
    {
        public void Start()
        {
            ServiceManager.Open();
        }

        public void Stop()
        {
            ServiceManager.Close();
        }
    }
}
