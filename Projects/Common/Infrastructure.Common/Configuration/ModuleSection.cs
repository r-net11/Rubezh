using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Infrastructure.Common.Configuration
{
	public class ModuleSection : ConfigurationSection
	{
		[ConfigurationProperty("", IsRequired = false, IsKey = false, IsDefaultCollection = true)]
		public ModuleCollection Modules
		{
			get { return ((ModuleCollection)(base[""])); }
			set { base[""] = value; }
		}
	}
}
