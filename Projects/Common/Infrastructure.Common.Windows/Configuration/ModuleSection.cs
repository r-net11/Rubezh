using System.Configuration;

namespace Infrastructure.Common.Windows.Configuration
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