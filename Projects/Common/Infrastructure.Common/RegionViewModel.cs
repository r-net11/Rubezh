using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common
{
    public abstract class RegionViewModel :
            BaseViewModel,
            IViewPart
    {
        protected RegionViewModel()
        {
        }

        //public abstract void Dispose();
        public virtual void OnShow()
        {
        }
        public virtual void OnHide()
        {
        }
    }
}
