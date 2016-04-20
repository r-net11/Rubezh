using System.Configuration;

namespace Infrastructure.Common.Windows.Configuration
{
	[ConfigurationCollection(typeof(ModuleElement), CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
	public class ModuleCollection : ConfigurationElementCollection
	{
		internal const string ItemPropertyName = "module";

		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.BasicMapAlternate; }
		}

		protected override string ElementName
		{
			get { return ItemPropertyName; }
		}

		protected override bool IsElementName(string elementName)
		{
			return (elementName == ItemPropertyName);
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ModuleElement)element).AssemblyFile;
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new ModuleElement();
		}

		public override bool IsReadOnly()
		{
			return false;
		}
	}
}
