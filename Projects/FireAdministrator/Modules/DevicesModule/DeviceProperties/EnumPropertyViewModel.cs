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
        public EnumPropertyViewModel(Firesec.Metadata.configDrvPropInfo propertyInfo, Device device)
            : base(propertyInfo, device)
        {
            var property = device.Properties.FirstOrDefault(x => x.Name == propertyInfo.name);
            if (property != null)
            {
                _selectedValue = propertyInfo.param.FirstOrDefault(x => x.value == property.Value).name;
            }
            else
            {
                _selectedValue = propertyInfo.param.FirstOrDefault(x => x.value == propertyInfo.@default).name;
            }
        }

        public List<string> Values
        {
            get
            {
                List<string> values = new List<string>();
                foreach (var propertyParameter in _propertyInfo.param)
                {
                    values.Add(propertyParameter.name);
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
