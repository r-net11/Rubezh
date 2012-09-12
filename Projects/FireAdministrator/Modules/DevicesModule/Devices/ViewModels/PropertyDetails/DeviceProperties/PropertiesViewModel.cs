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
        public static DevicesViewModel Context { get; private set; }

        public PropertiesViewModel(DevicesViewModel deviceViewModel)
        {
            Context = deviceViewModel;
        }

		public PropertiesViewModel(Device device)
		{
            OneCommand = new RelayCommand(OnOne);
            TwoCommand = new RelayCommand(OnTwo);
			Device = device;
			StringProperties = new List<StringPropertyViewModel>();
			BoolProperties = new List<BoolPropertyViewModel>();
			EnumProperties = new List<EnumPropertyViewModel>();

			foreach (var driverProperty in device.Driver.Properties)
			{
				if (driverProperty.IsHidden)
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

	    private bool aUParameterVis = true;
        public bool AUParameterVis
	    {
            get
            {
                return aUParameterVis;
            }
            set 
            {
                aUParameterVis = value;
                OnPropertyChanged("AUParameterVis");
            }
	    }
        private bool choise;
	    public bool Choise
	    {
            get
            {
                bool choise1 = (StringProperties.FirstOrDefault(x => x.IsAUParameter) == null) &&
                               (BoolProperties.FirstOrDefault(x => x.IsAUParameter) == null) &&
                               (EnumProperties.FirstOrDefault(x => x.IsAUParameter) == null);
                bool choise2 = (StringProperties.FirstOrDefault(x => x.IsAUParameter == false) == null) &&
                               (BoolProperties.FirstOrDefault(x => x.IsAUParameter == false) == null) &&
                               (EnumProperties.FirstOrDefault(x => x.IsAUParameter == false) == null);
                if (choise1)
                {
                    choise = false;
                    AUParameterVis = false;
                }
                if (choise2)
                {
                    choise = false;
                    AUParameterVis = true;
                }
                if(choise1&&choise2)
                {
                    choise = false;
                    AUParameterVis = false;
                }
                if(!choise1&&!choise2)
                {
                    choise = true;
                    AUParameterVis = true;
                }
                return choise;
            }
            set
            {
                choise = value;
                OnPropertyChanged("Choise");
            }
	    }
	 
        public RelayCommand OneCommand { get; private set; }
        private void OnOne()
        {
            AUParameterVis = true;
        }
        public RelayCommand TwoCommand { get; private set; }
        private void OnTwo()
        {
            AUParameterVis = false;
        }
    }
}