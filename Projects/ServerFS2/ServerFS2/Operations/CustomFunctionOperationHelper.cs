using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FS2Api;

namespace ServerFS2.Operations
{
	public static class CustomFunctionOperationHelper
	{
		public static void Execute(Device device, string commandName)
		{
			throw new FS2Exception("Функция пока не реализована");
		}
	}
}