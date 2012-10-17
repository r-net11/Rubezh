using System.Collections.Generic;
using System.Linq;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.DeviceProperties
{
	public class PropertiesViewModel : BaseViewModel
	{
		public Device Device { get; private set; }
		public List<StringPropertyViewModel> StringProperties { get; set; }
		public List<BoolPropertyViewModel> BoolProperties { get; set; }
		public List<EnumPropertyViewModel> EnumProperties { get; set; }

		public PropertiesViewModel(Device device)
		{
            ParamVisCommand = new RelayCommand(OnParamVis);
            PropVisCommand = new RelayCommand(OnPropVis);
			Device = device;
			StringProperties = new List<StringPropertyViewModel>();
			BoolProperties = new List<BoolPropertyViewModel>();
			EnumProperties = new List<EnumPropertyViewModel>();
			foreach (var driverProperty in device.Driver.Properties)
			{
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

		public bool IsMonitoringDisabled
		{
			get { return Device.IsMonitoringDisabled; }
			set
			{
				Device.IsMonitoringDisabled = value;
				OnPropertyChanged("IsMonitoringDisabled");
				ServiceFactory.SaveService.DevicesChanged = true;
			}
		}

	    private bool parameterVis;
        public bool ParameterVis
	    {
            get
            {
                return parameterVis;
            }
            set 
            {
                parameterVis = value;
                OnPropertyChanged("ParameterVis");
            }
	    }
        private bool choise = true;
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
        private void OnParamVis()
        {
            ParameterVis = true;
        }
        public RelayCommand PropVisCommand { get; private set; }
        private void OnPropVis()
        {
            ParameterVis = false;
        }
    }
}