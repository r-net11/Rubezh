using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace AdministratorTestClientFS2.ViewModels
{
    public class PropertiesViewModel : BaseViewModel
    {
        private bool _isAuParametersReady = true;
        private bool choise = true;
        private bool parameterVis;

        public PropertiesViewModel(DevicesViewModel deviceViewModel)
        {
            Context = deviceViewModel;
        }

        public PropertiesViewModel(Device device)
        {
            ParamVisCommand = new RelayCommand(OnParamVis);
            PropVisCommand = new RelayCommand(OnPropVis);
            Device = device;
            StringProperties = new List<StringPropertyViewModel>();
            BoolProperties = new List<BoolPropertyViewModel>();
            EnumProperties = new List<EnumPropertyViewModel>();
            if (device.Driver == null)
                return;
            foreach (var driverProperty in device.Driver.Properties)
            {
                if (device.Driver.DriverType == DriverType.Exit)
                    continue;

                if (device.Driver.DriverType == DriverType.PumpStation && driverProperty.Name == "AllowControl")
                    continue;

                if (driverProperty.IsHidden || driverProperty.IsControl)
                    continue;

                switch (driverProperty.DriverPropertyType)
                {
                    case DriverPropertyTypeEnum.EnumType:
                        EnumProperties.Add(new EnumPropertyViewModel(driverProperty, device));
                        break;

                    case DriverPropertyTypeEnum.StringType:
                    case DriverPropertyTypeEnum.IntType:
                    case DriverPropertyTypeEnum.ByteType:
                        StringProperties.Add(new StringPropertyViewModel(driverProperty, device));
                        break;

                    case DriverPropertyTypeEnum.BoolType:
                        BoolProperties.Add(new BoolPropertyViewModel(driverProperty, device));
                        break;
                }
            }
        }

        public Device Device { get; private set; }
        public List<StringPropertyViewModel> StringProperties { get; set; }
        public List<BoolPropertyViewModel> BoolProperties { get; set; }
        public List<EnumPropertyViewModel> EnumProperties { get; set; }

        public static DevicesViewModel Context { get; private set; }

        public bool IsMonitoringDisabled
        {
            get { return Device.IsMonitoringDisabled; }
            set
            {
                Device.IsMonitoringDisabled = value;
                OnPropertyChanged("IsMonitoringDisabled");
            }
        }

        public bool ParameterVis
        {
            get { return parameterVis; }
            set
            {
                parameterVis = value;
                OnPropertyChanged("ParameterVis");
            }
        }

        public bool Choise
        {
            get
            {
                bool choise1 = (StringProperties.FirstOrDefault(x => x.IsAUParameter) == null) &&
                               (BoolProperties.FirstOrDefault(x => x.IsAUParameter) == null) &&
                               (EnumProperties.FirstOrDefault(x => x.IsAUParameter) == null);
                if (choise1)
                {
                    choise = false;
                    ParameterVis = false;
                }
                return choise;
            }
            set
            {
                choise = value;
                OnPropertyChanged("Choise");
            }
        }

        public RelayCommand ParamVisCommand { get; private set; }

        public RelayCommand PropVisCommand { get; private set; }

        public bool IsAuParametersReady
        {
            get { return _isAuParametersReady; }
            set
            {
                _isAuParametersReady = value;
                OnPropertyChanged("IsAuParametersReady");
            }
        }

        private void OnParamVis()
        {
            ParameterVis = true;
        }

        private void OnPropVis()
        {
            ParameterVis = false;
        }
    }
}