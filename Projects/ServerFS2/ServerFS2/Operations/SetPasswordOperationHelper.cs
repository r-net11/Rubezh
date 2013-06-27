using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ServerFS2.Operations
{
	public static class SetPasswordOperationHelper
	{
		public static void SetPassword(Device device, DevicePasswordType devicePasswordType, string password)
		{
			for (int i = password.Length; i < 6; i++)
			{
				password += 'F';
			}
			password = password.Insert(2, " ");
			password = password.Insert(5, " ");
			var bytes = password.Split().Select(t => byte.Parse(t, NumberStyles.AllowHexSpecifier)).ToList();
			if (devicePasswordType == DevicePasswordType.Administrator)
				USBManager.Send(device, 0x02, 0x52, BytesHelper.IntToBytes(0x0A), 0x02, bytes);
			if (devicePasswordType == DevicePasswordType.Installator)
				USBManager.Send(device, 0x02, 0x52, BytesHelper.IntToBytes(0x07), 0x02, bytes);
			if (devicePasswordType == DevicePasswordType.Operator)
				USBManager.Send(device, 0x02, 0x52, BytesHelper.IntToBytes(0x04), 0x02, bytes);
		}
	}
}
