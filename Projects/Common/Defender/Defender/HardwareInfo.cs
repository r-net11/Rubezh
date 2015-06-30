using System.Collections;
using System.Management;

namespace Defender
{
    public static class HardwareInfo
    {
        static string _cpuId;
        public static string CpuId
        {
            get
            {
                if (_cpuId == null)
                    _cpuId = GetFromWmi("Select ProcessorID From Win32_processor", "ProcessorID");
                return _cpuId;
            }
        }

        static string _hardDiskId;
        public static string HardDiskId
        {
            get
            {
                if (_hardDiskId == null)
                    _hardDiskId = GetFromWmi("Select VolumeSerialNumber From Win32_LogicalDisk Where DeviceID = \"C:\"", "VolumeSerialNumber");
                return _hardDiskId;
            }
        }

        static string GetFromWmi(string query, string column, string scope = null)
        {
            try
            {
                IEnumerator wmi = new ManagementObjectSearcher(scope, query).Get().GetEnumerator();
                return wmi.MoveNext() ? ((ManagementObject)wmi.Current)[column].ToString() : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
