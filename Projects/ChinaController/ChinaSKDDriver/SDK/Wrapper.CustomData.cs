using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public string GetCustomData()
		{
			NativeWrapper.WRAP_CustomData customData;
			return NativeWrapper.WRAP_GetCustomData(LoginID, out customData) ? customData.CustomData : null;
		}

		public bool SetCustomData(string data)
		{
			if (data == null)
				return false;

			var customData = new NativeWrapper.WRAP_CustomData { CustomDataLength = data.Length, CustomData = data };
			return NativeWrapper.WRAP_SetCustomData(LoginID, ref customData);
		}
	}
}