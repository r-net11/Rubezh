using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ServiceApi
{
    [DataContract]
    public class ValidationError
    {
        public ValidationError(string error, Level level)
        {
            Error = error;
            Level = level;
        }

        [DataMember]
        public string Error { get; set; }

        [DataMember]
        public Level Level { get; set; }
    }

    public enum Level
    {
        Critical,
        Normal
    }
}
