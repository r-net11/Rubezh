using Microsoft.Practices.Prism.Events;
using System.Collections.Generic;
using RubezhAPI.SKD;

namespace Infrastructure.Events
{
	public class SelectOrganisationsEvent : CompositePresentationEvent<SelectOrganisationsEventArg>
	{
	}

	public class SelectOrganisationsEventArg
	{
		public bool Cancel { get; set; }
		public List<Organisation> Organisations { get; set; }
	}
}