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