namespace FiresecAPI.GK
{
	public class EventName
	{
		public EventName(string name, XStateClass stateClass, string description)
		{
			Name = name;
			StateClass = stateClass;
			Description = description;
		}

		public string Name { get; private set; }
		public string Description { get; private set; }
		public XStateClass StateClass { get; private set; }
	}
}