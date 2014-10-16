using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Infrastructure.Common.Services.Layout
{
    public interface ILayoutPartContent
    {
        string Title { get; }
        string IconSource { get; }
        event EventHandler TitleChanged;
        event EventHandler IconChanged;
    }
}
