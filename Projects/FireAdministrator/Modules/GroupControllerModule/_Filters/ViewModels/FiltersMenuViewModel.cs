using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
    public class FiltersMenuViewModel : BaseViewModel
    {
        public FiltersMenuViewModel(FiltersViewModel context)
        {
            Context = context;
        }

        public FiltersViewModel Context { get; private set; }
    }
}