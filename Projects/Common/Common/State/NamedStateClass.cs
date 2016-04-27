using FiresecAPI.GK;

namespace Common
{
	public class NamedStateClass
	{
		public NamedStateClass()
		{
			StateClass = XStateClass.No;
            Name = Resources.Language.State.NamedStateClass.Name;
		}

		public XStateClass StateClass { get; set; }

		public string Name { get; set; }
	}
}