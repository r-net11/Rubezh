using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
    public class FSShleifViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public int FSShleifNo { get; set; }

        public string KAUName { get; set; }
        public int KAUShleifNo { get; set; }
    }
}