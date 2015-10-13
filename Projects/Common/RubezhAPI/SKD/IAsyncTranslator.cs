using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RubezhAPI.SKD
{
    public interface IAsyncTranslator<TFilter>
    {
        event Action<DbCallbackResult> PortionReady;
        void BeginGet(TFilter filter);
    }
}
