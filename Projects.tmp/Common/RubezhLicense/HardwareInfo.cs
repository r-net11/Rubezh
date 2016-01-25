using System.Collections;

namespace RubezhLicense
{
    static class HardwareInfo
    {
        static string _cpuId;
        public static string CpuId
        {
            get
            {
                if (_cpuId == null)
					_cpuId = "9876543210"; //TODO: get cpuId (linux)
                return _cpuId;
            }
        }

        static string _hardDiskId;
        public static string HardDiskId
        {
            get
            {
                if (_hardDiskId == null)
					_hardDiskId = "0123456789"; //TODO: get cpuId (linux)
                return _hardDiskId;
            }
        }
    }
}
