using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FS2Api
{
	public class FS2Exception : Exception
	{
		public FS2Exception(string message)
			:base(message)
		{

		}
	}
}