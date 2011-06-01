using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient;

namespace DevicesModule.PropertyBindings
{
    public class BoolProperty : BaseProperty
    {
        public BoolProperty(Firesec.Metadata.propInfoType propertyInfo, Device device)
            : base(propertyInfo, device)
        {
            var property = device.Properties.FirstOrDefault(x => x.Name == propertyInfo.name);
            if (property != null)
                _isChecked = property.Value == "1" ? true : false;
            else
                _isChecked = (propertyInfo.@default == "1") ? true : false;
        }

        bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
                Save(value ? "1" : "0");
            }
        }
    }
}
