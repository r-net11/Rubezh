using System;

namespace FiresecAPI
{
	public class FiresecException : Exception
	{
		public FiresecException(string message)
			: base(message)
		{
		}
	}
}