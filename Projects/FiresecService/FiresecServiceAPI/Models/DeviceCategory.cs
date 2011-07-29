using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    public enum DeviceCategoryType
    {
        Other,
        Device,
        Sensor,
        Effector,
        Communication,
        RemoteServer,
        None
    }

    [DataContract]
    public class DeviceCategory
    {
        [DataMember]
        public int Id { get; set; }

        public DeviceCategoryType DeviceCategoryType
        {
            get { return (DeviceCategoryType) Id; }
        }

        public static bool operator ==(DeviceCategory value1, DeviceCategory value2)
        {
            return value1.Id == value2.Id;
        }

        public static bool operator !=(DeviceCategory value1, DeviceCategory value2)
        {
            return value1.Id != value2.Id;
        }

        public override bool Equals(object obj)
        {
            return ((DeviceCategory) obj).Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
