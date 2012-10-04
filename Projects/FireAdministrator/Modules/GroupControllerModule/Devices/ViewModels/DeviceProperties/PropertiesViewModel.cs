using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class PropertiesViewModel : BaseViewModel
	{
		public XDevice XDevice { get; private set; }
		public List<StringPropertyViewModel> StringProperties { get; set; }
		public List<ShortPropertyViewModel> ShortProperties { get; set; }
		public List<BoolPropertyViewModel> BoolProperties { get; set; }
		public List<EnumPropertyViewModel> EnumProperties { get; set; }

		public PropertiesViewModel(XDevice xDevice)
		{
            OneCommand = new RelayCommand(OnOne);
            TwoCommand = new RelayCommand(OnTwo);
			XDevice = xDevice;
			StringProperties = new List<StringPropertyViewModel>();
			ShortProperties = new List<ShortPropertyViewModel>();
			BoolProperties = new List<BoolPropertyViewModel>();
			EnumProperties = new List<EnumPropertyViewModel>();

			if (xDevice != null)
				foreach (var driverProperty in xDevice.Driver.Properties)
				{
					switch (driverProperty.DriverPropertyType)
					{
						case XDriverPropertyTypeEnum.EnumType:
							EnumProperties.Add(new EnumPropertyViewModel(driverProperty, xDevice));
							break;

						case XDriverPropertyTypeEnum.StringType:
							StringProperties.Add(new StringPropertyViewModel(driverProperty, xDevice));
							break;

						case XDriverPropertyTypeEnum.IntType:
							ShortProperties.Add(new ShortPropertyViewModel(driverProperty, xDevice));
							break;

						case XDriverPropertyTypeEnum.BoolType:
							BoolProperties.Add(new BoolPropertyViewModel(driverProperty, xDevice));
							break;
					}
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
                bool choise1 = (StringProperties.Count == 0) &&
                               (BoolProperties.Count == 0) &&
                               (EnumProperties.Count == 0);

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

        public RelayCommand OneCommand { get; private set; }
        private void OnOne()
        {
            ParameterVis = true;
        }
        public RelayCommand TwoCommand { get; private set; }
        private void OnTwo()
        {
            ParameterVis = false;
        }
	}
}