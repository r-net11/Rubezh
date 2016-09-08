using Localization.Common.Common;
using StrazhAPI.GK;

namespace Common
{
	public class NamedStateClass
	{
		public NamedStateClass()
		{
			StateClass = XStateClass.No;
			Name = CommonResources.None;
		}

		public XStateClass StateClass { get; set; }

		public string Name { get; set; }
	}
}