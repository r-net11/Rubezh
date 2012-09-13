using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;

namespace DiagnosticsModule.ViewModels
{
    public class DiagnosticsViewModel : ViewPartViewModel
    {
        public DiagnosticsViewModel()
        {
            ShowImitatorCommand = new RelayCommand(OnShowImitator);
        }

        public RelayCommand ShowImitatorCommand { get; private set; }
        void OnShowImitator()
        {
        }
    }
}