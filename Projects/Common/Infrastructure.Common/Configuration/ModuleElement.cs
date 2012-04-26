using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Infrastructure.Common.Configuration
{
	public class ModuleElement : ConfigurationElement
	{
		[ConfigurationProperty("assemblyFile")]
		public string AssemblyFile
		{
			get { return (string)base["assemblyFile"]; }
			set { base["assemblyFile"] = value; }
		}
	}
}
