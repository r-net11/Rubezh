using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient;

namespace DevicesModule.DeviceProperties
{
    public class StringPropertyViewModel : BasePropertyViewModel
    {
        public StringPropertyViewModel(Firesec.Metadata.propInfoType propertyInfo, Device device) : base(propertyInfo, device)
        {
            var property = device.Properties.FirstOrDefault(x => x.Name == propertyInfo.name);
            if (property != null)
                _text = property.Value;
            else
                _text = propertyInfo.@default;
        }

        string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
                Save(value);
            }
        }
    }
}
