using System.Linq;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class BoolPropertyViewModel : BasePropertyViewModel
    {
        public BoolPropertyViewModel(XDriverProperty xDriverProperty, XDevice xDevice)
            : base(xDriverProperty, xDevice)
        {
            var property = xDevice.Properties.FirstOrDefault(x => x.Name == xDriverProperty.Name);
            if (property != null)
                _isChecked = property.Value == "1" ? true : false;
            else
                _isChecked = (xDriverProperty.Default == "1") ? true : false;
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