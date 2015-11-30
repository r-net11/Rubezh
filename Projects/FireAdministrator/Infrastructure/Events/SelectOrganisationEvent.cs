using Microsoft.Practices.Prism.Events;
using RubezhAPI.SKD;

namespace Infrastructure.Events
{
	public class SelectOrganisationEvent : CompositePresentationEvent<SelectOrganisationEventArg>
	{
	}

	public class SelectOrganisationEventArg
	{
		public bool Cancel { get; set; }
		public Organisation Organisation { get; set; }
	}
}