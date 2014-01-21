using System;

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