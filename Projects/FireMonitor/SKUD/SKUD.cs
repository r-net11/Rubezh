using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SKUD
{
	public class SKUD
	{
		public DataAccess.SKUDDataContext Context { get; private set; }

		public SKUD()
		{
			Context = new DataAccess.SKUDDataContext();
		}
	}
}
