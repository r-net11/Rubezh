using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;

namespace VideoModule.ViewModels
{
    public class ZoneSelectationViewModel : SaveCancelDialogContent
    {
        public ZoneSelectationViewModel()
        {
            Title = "Выбор зоны";
        }

        protected override void Save(ref bool cancel)
        {
        }
    }
}