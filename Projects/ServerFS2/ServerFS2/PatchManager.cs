using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace ServerFS2
{
	public static class PatchManager
	{
		public static void Patch()
		{
			try
			{
				Patch1();
			}
			catch (Exception e)
			{
				Logger.Error(e, "PatchManager.Patch");
			}
		}

		static void Patch1()
		{

		}
	}
}