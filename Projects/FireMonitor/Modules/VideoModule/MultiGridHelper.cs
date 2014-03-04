using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoModule
{
    public static class MultiGridHelper
    {
        public static Dictionary<string, Guid> _2X2Collection;
        public static Dictionary<string, Guid> _1X7Collection;
        public static Dictionary<string, Guid> _3X3Collection;
        public static Dictionary<string, Guid> _4X4Collection;
        public static Dictionary<string, Guid> _6X6Collection;

        static MultiGridHelper()
        {
            _2X2Collection = new Dictionary<string, Guid>();
            _1X7Collection = new Dictionary<string, Guid>();
            _3X3Collection = new Dictionary<string, Guid>();
            _4X4Collection = new Dictionary<string, Guid>();
            _6X6Collection = new Dictionary<string, Guid>();
        }
    }
}
