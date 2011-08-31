using FiresecAPI.Models;

namespace FiresecClient.Validation
{
    public class DeviceError : BaseError
    {
        public DeviceError(Device device, string error, ErrorLevel level)
            : base(error, level)
        {
            Device = device;
        }

        public Device Device { get; set; }
    }
}