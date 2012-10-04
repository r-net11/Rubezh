using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Firesec.Imitator;

namespace DiagnosticsModule.ViewModels
{
    public class ImitatorViewModel : BaseViewModel
    {
        public ImitatorViewModel()
        {
            ShowImitatorCommand = new RelayCommand(OnShowImitator);
        }

        public RelayCommand ShowImitatorCommand { get; private set; }
        void OnShowImitator()
        {
			ImitatorService.Show();
        }
    }
}