using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerFS2
{
	public class FS2USBException : Exception
	{
		public FS2USBException(string message)
			: base(message)
		{

		}
	}
}