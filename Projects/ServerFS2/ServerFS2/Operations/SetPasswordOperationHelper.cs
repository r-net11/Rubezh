using System.Globalization;
using System.Linq;
using FiresecAPI.Models;

namespace ServerFS2.Operations
{
	public static class SetPasswordOperationHelper
	{
		public static void SetPassword(Device device, DevicePasswordType devicePasswordType, string password)
		{
			if (password == null)
				password = "";
			for (int i = password.Length; i < 6; i++)
			{
				password += 'F';
			}
			password = password.Insert(2, " ");
			password = password.Insert(5, " ");
			var bytes = password.Split().Select(t => byte.Parse(t, NumberStyles.AllowHexSpecifier)).ToList();

			var passwordTypeNo = GetPasswordTypeNo(devicePasswordType);
			USBManager.Send(device, "Запись пароля", 0x02, 0x52, BytesHelper.IntToBytes(passwordTypeNo), 0x02, bytes);
		}

		static int GetPasswordTypeNo(DevicePasswordType devicePasswordType)
		{
			switch(devicePasswordType)
			{
				case DevicePasswordType.Administrator:
					return 0x0A;
				case DevicePasswordType.Installator:
					return 0x07;
				case DevicePasswordType.Operator:
					return 0x04;
			}
			return 0;
		}
	}
}