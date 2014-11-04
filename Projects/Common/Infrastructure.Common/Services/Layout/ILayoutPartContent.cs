using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using FiresecAPI.Automation;

namespace Infrastructure.Common.Services.Layout
{
    public interface ILayoutPartContent
    {
        ILayoutPartContainer Container { get; }
        void SetLayoutPartContainer(ILayoutPartContainer container);
    }
}
