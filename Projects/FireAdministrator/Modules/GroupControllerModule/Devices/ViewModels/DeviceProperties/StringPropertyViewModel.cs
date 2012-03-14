using System.Linq;
using GroupControllerModule.Models;

namespace GroupControllerModule.ViewModels
{
    public class StringPropertyViewModel : BasePropertyViewModel
    {
        public StringPropertyViewModel(XDriverProperty xDriverProperty, XDevice xDevice)
            : base(xDriverProperty, xDevice)
        {
            var property = xDevice.Properties.FirstOrDefault(x => x.Name == xDriverProperty.Name);
            if (property != null)
                _text = property.Value;
            else
                _text = xDriverProperty.Default;
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