using GroupControllerModule.Models;
using Infrastructure.Common;
using XFiresecAPI;
using GroupControllerModule.Converter;

namespace GroupControllerModule.ViewModels
{
    public class BinObjectViewModel : BaseViewModel
    {
        public string Caption { get; set; }
        public string ImageSource { get; set; }
        public int Level { get; set; }
        public DeviceBinaryFormatter DeviceBinaryFormatter { get; set; }
    }
}