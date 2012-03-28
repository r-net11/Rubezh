using System.Linq;
using GroupControllerModule.Models;
using XFiresecAPI;

namespace GroupControllerModule.ViewModels
{
    public class StringPropertyViewModel : BasePropertyViewModel
    {
        public StringPropertyViewModel(XDriverProperty xDriverProperty, XDevice xDevice)
            : base(xDriverProperty, xDevice)
        {
            var property = xDevice.Properties.FirstOrDefault(x => x.Name == xDriverProperty.Name);
            if (property != null)
                _text = (string)property.Value;
            else
                _text = (string)xDriverProperty.Default;
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