using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licensing.Generator
{
	public abstract class GeneratorBase
	{
		public string GenerateLicense()
		{
			GenerateLicenseBASE64String
		}

		protected abstract string GenerateLicenseBASE64String();

		protected abstract string SignXML();
	}
}
