using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure
{
    public interface ILayoutService
    {
        void Show(IViewPart model);
        void Close();
    }
}
