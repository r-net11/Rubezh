using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
    public class _2X2GridViewModel: BaseViewModel
    {
        public List<LayoutPartCameraViewModel> LayoutPartCameraViewModels { get; private set; }

        public _2X2GridViewModel()
        {
            LayoutPartCameraViewModels = new List<LayoutPartCameraViewModel>();
        }
    }
}
