using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.SKD
{
    public interface IAsyncTranslator<TFilter>
    {
        event Action<DbCallbackResult> PortionReady;
        void BeginGet(TFilter filter);
    }
}
