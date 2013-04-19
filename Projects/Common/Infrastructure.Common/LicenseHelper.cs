using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using Aladdin.HASP;

namespace Infrastructure.Common
{
	public static class LicenseHelper
	{
		static string _vendorCode =
			"hSxuvX+4Ik4ehlUbRjIi8NVx128oM0LHXfM8gyi5P+uUYY6yXKu798W1a7gFrjiAbLSg1taawgkszHhG" +
			"zW0nlUzPN19fkiyseshhe7ag1ZChQihaMgBXyJfDOlC24bz8F01H7didmW/kZIbXC38CA2CQ4VPosoC4" +
			"3Lxq06xEBckzM9EQnTBF5tWimUhu4Cdvh4xkB57jqjmvthXkia7RYTwaVv7ZmP5kzadxz//lLLOhgBuD" +
			"j+/h6SgUy9z5vcqb8MJFXtJOf0F/u+C5NKN1wHb9l7EPuFagb+u1/tZrWdDGBDpp6VqRC2F6/u9OElFQ" +
			"iDj3aPhDxtumE+LPt7Rt6ErpYXGzmWOnVzHGPljGfLVbacNLMDM2uHyKZTduTPvVOKjG+XbtnnJXqmIG" +
			"Y/tdzthAl89D4NyMwK5buGgEKibuzI1fK6xjnMNn/s/oZCsQxP4GPZGasWIqBHy44RtkXvIi/1E7m//w" +
			"zpXwgAimx8ZeFAGelD6Af4eaHVg0Bo0A+JBDlrRhQszUNGuJDiZY8NAHq34JITGQNALrMsdrc7KWibQg" +
			"jTfBAjsfsHRJPzJKvD0I0vrlnTs/HvQIUuX9mJh3D+/NlIGpVx7KmfssRBvoTUipicyYc0M4DS2mffLw" +
			"IqXgLzS+PFZbi0abwaikNk4Gfx4VlbsLPeH+Jm/3RCrmo29f5thYxbsAg9fvTLC2gfSxfAafzvrRlr5Q" +
			"nIF+jhEsDXgGoMWTeT/ogFLMlQlSp9WPzbZFWRFrg5FyUr805pgrCbD4n/mOMbOqlJ8E7LFr/MWjaUQQ" +
			"F4cgpYyaSSojCNmY1dC9aFUd9jbpEm1ucKTZvaL0IDrz1cZ92OxkV8AmPkW2KeIdq8MkPyTDK9DyYAz2" +
			"Nqwe4FFLz8dvlUjtoQrSW5xMYYT+MoEHFJfZ1yE8nd2QUmni7/OTTYyhaF4=";

        [DllImport("hasp_windows.dll")]
        public static extern uint hasp_login(uint hasp_feature_t, IntPtr hasp_vendor_code_t, uint handle = new uint());

        [DllImport("hasp_windows.dll")]
        public static extern uint hasp_logout(uint handle);

        [HandleProcessCorruptedStateExceptions]
		public static bool CheckLicense(int haspFeatureId)
		{
			return true;
#if DEBUG
			//return true;
#endif

            _vendorCode =
                @"P0onHrylb96psVS/AEv5ovZA2BOsD9U9Ne+NwH8HiACds13rO5FG8i9ILS7pQcYvu2Td+XmkIOcvPOmPQwtTVHGIdd5U60ZKQMGlkpk00EUoOOlDrNhiz3SwQPSrjmXH5TqoR0mNccLyiPqaJWcrrzOYL66SPHcgh1pnMx8f6V6GXEpS6RKfrQHxJLGE/uPkFS8so6iEoE5yhUxKxM/CfTGzEN+R56j30wCWwTrc/lsCS8Psl6m66umzEMbxpcE995cSCdIGxdJRhEHVy4ubTm18t2x1c8+NoffnpOJCRq2EzHMyuuc1O0jkHhQdshid4YlasUH5Z2zR2snLKbvpJVqMOQq0P2bkuGRjA3FPnrfXZRlAOBDrA959C28T2s9wD1pZ8x55pSuostd0jXdPTDoGqFbN7/2stplg3j+Ycw0fLYQ1BhfbfT+HJxzaf/kHxcdE2xQVkB1YH/z7hF8RV7rjUIhBNajoeJxgow4SRUSjDGqWCJH0Jf1wH8Vxnja+pCCj8FXCNt+WLTlKX75eRmI+LuP7ZZK2U1Lk9B4Hp7+jrFXJk1qDQ43kZRIvSE7MODhoUQHamFsvSkZeG9Sd4h5HTqYl/GYEK95Zusq3VMkwMxLJ4LvULHUqjzv0FANNont7Y1JOwm1KP1QRSNwTQfIw0GoHjHbi8sEYBDHslYTjEQIUSoxHPhbCnwjNepTBo0n8SdCX+VdJhCxWw/D8+PAlF5r7eA==";
            Encoding enc = Encoding.GetEncoding("windows-1251");
			var status = hasp.Login(_vendorCode);
            GCHandle pinnedArray = GCHandle.Alloc(bytesarray, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();
            try
            {
                var statusNew = hasp_login(4294901762, pointer, 0);
            }
            catch(AccessViolationException e)
            {
                pinnedArray.Free();
                var status = hasp_logout(0);
                return true;
            }
            pinnedArray.Free();
			return false;
		}
	}
}