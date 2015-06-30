using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class UserChangedEvent : CompositePresentationEvent<UserChangedEventArgs>
	{
	}

	public class UserChangedEventArgs
	{
		public bool IsReconnect { get; set; }
		public string OldName { get; set; }
		public string NewName { get; set; }
	}
}