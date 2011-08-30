using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public static class GuidHelper
    {
        public static string ToString(Guid guid)
        {
            if (guid == Guid.Empty)
                return null;
            return guid.ToString();
        }

        public static Guid ToGuid(string val)
        {
            if (val == null)
                return Guid.Empty;
            return new Guid(val);
        }
    }
}
