using Microsoft.Practices.Prism.Events;
using RubezhAPI.Models;

namespace Infrastructure.Events
{
	public class DeleteUserEvent: CompositePresentationEvent<User>
	{
	}
}