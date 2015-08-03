using System;
using Microsoft.Practices.Prism.Events;

namespace SKDModule.Events
{
	class DeleteCardEvent : CompositePresentationEvent<Guid>
	{
	}
}