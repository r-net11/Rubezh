using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;

namespace FiresecClient.Validation
{
    public class BaseError
    {
        public BaseError(string error, ErrorLevel level)
        {
            Error = error;
            Level = level;
        }

        public string Error { get; set; }
        public ErrorLevel Level { get; set; }
    }
}
