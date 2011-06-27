using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient;
using FiresecClient.Models;

namespace DevicesModule.DeviceProperties
{
    public class EnumPropertyViewModel : BasePropertyViewModel
    {
        public EnumPropertyViewModel(DriverProperty driverProperty, Device device)
            : base(driverProperty, device)
        {
            var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
            if (property != null)
            {
                _selectedValue = driverProperty.Parameters.FirstOrDefault(x => x.Value == property.Value).Name;
            }
            else
            {
                _selectedValue = driverProperty.Parameters.FirstOrDefault(x => x.Value == driverProperty.Default).Name;
            }
        }

        public List<string> Values
        {
            get
            {
                List<string> values = new List<string>();
                foreach (var propertyParameter in _driverProperty.Parameters)
                {
                    values.Add(propertyParameter.Name);
                }
                return values;
            }
        }

        string _selectedValue;
        public string SelectedValue
        {
            get { return _selectedValue; }
            set
            {
                _selectedValue = value;
                OnPropertyChanged("SelectedValue");
                Save(value);
            }
        }
    }
}
