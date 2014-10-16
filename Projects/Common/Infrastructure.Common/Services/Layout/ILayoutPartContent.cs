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
        string ImageSource { get; }
        event EventHandler TitleChanged;
        event EventHandler ImageChanged;
    }
}
