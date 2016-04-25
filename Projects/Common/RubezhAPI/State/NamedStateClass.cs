using RubezhAPI.GK;

namespace RubezhAPI
{
	public class NamedStateClass
	{
		public NamedStateClass()
		{
			StateClass = XStateClass.No;
			Name = "Нет";
		}

		public XStateClass StateClass { get; set; }
		public string Name { get; set; }
	}
}