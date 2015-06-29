using Defender;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace FiresecService.ViewModels
{
    public class LicenseViewModel : BaseViewModel
    {
        License _license;

        public InitialKey InitialKey
        {
            get { return _license.InitialKey; }
        }
 
        ObservableCollection<LicenseParameter> _Parameters;
        public ObservableCollection<LicenseParameter> Parameters
        {
            get { return _Parameters; }
            set
            {
                _Parameters = value;
                OnPropertyChanged(() => Parameters);
            }
        }

        public LicenseViewModel(License license)
        {
            _license = license;

            if (_license == null)
                Parameters = new ObservableCollection<LicenseParameter>();
            else
                Parameters = new ObservableCollection<LicenseParameter>(_license.Parameters);
        }
    }
}
