using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Infrastructure.Common
{
    public static class LicenseHelper
    {
        static string _vendorCode = @"P0onHrylb96psVS/AEv5ovZA2BOsD9U9Ne+NwH8HiACds13rO5FG8i9ILS7pQcYvu2Td+XmkIOcvPOmPQwtTVHGIdd5U60ZKQMGlkpk00EUoOOlDrNhiz3SwQPSrjmXH5TqoR0mNccLyiPqaJWcrrzOYL66SPHcgh1pnMx8f6V6GXEpS6RKfrQHxJLGE/uPkFS8so6iEoE5yhUxKxM/CfTGzEN+R56j30wCWwTrc/lsCS8Psl6m66umzEMbxpcE995cSCdIGxdJRhEHVy4ubTm18t2x1c8+NoffnpOJCRq2EzHMyuuc1O0jkHhQdshid4YlasUH5Z2zR2snLKbvpJVqMOQq0P2bkuGRjA3FPnrfXZRlAOBDrA959C28T2s9wD1pZ8x55pSuostd0jXdPTDoGqFbN7/2stplg3j+Ycw0fLYQ1BhfbfT+HJxzaf/kHxcdE2xQVkB1YH/z7hF8RV7rjUIhBNajoeJxgow4SRUSjDGqWCJH0Jf1wH8Vxnja+pCCj8FXCNt+WLTlKX75eRmI+LuP7ZZK2U1Lk9B4Hp7+jrFXJk1qDQ43kZRIvSE7MODhoUQHamFsvSkZeG9Sd4h5HTqYl/GYEK95Zusq3VMkwMxLJ4LvULHUqjzv0FANNont7Y1JOwm1KP1QRSNwTQfIw0GoHjHbi8sEYBDHslYTjEQIUSoxHPhbCnwjNepTBo0n8SdCX+VdJhCxWw/D8+PAlF5r7eA==";

        [DllImport("hasp_windows.dll")]
        public static extern uint hasp_login(uint hasp_feature_t, IntPtr hasp_vendor_code_t, uint handle = new uint());

        [DllImport("hasp_windows.dll")]
        public static extern uint hasp_logout(uint handle);

        [HandleProcessCorruptedStateExceptions]
        public static bool CheckLicense(bool isMultiClient)
        {
#if DEBUG
			return true;
#endif
            var encoding = Encoding.GetEncoding("windows-1251");
            byte[] bytesarray = encoding.GetBytes(_vendorCode);
            GCHandle pinnedArray = GCHandle.Alloc(bytesarray, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();
			uint statusNew = 0;
            try
            {
                statusNew = hasp_login(4294901762, pointer, 0);
                if ((isMultiClient) && (statusNew != 0x07))
                    return true;
            }
            catch(AccessViolationException e)
            {
                pinnedArray.Free();
                var status = hasp_logout(0);
                return true;
            }
			pinnedArray.Free();
			if (statusNew == 0)
				return true;
			return false;
		}

		public static string GetHaspInfo()
		{
			var hasLicense = CheckLicense(false);
			var hasLicenseForMulticlient = CheckLicense(true);
			var result = "Параметры HASP ключа, подключенного к этому компьютеру" + "\n";
			if (hasLicense)
				result = "Ключ присутствует" + "\n";
			else
				result = "Ключ отсутствует" + "\n";

			if (hasLicenseForMulticlient)
				result += "Управление доступно";// + "\n" + "Мультисерверная ОЗ доступна";
			else
				result += "Управление не доступно";// + "\n" + "Мультисерверная ОЗ не доступна";
			return result;
		}
	}
}