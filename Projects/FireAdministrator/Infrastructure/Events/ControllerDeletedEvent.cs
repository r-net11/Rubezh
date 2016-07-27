using System;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	/// <summary>
	/// Описывает событие, происходящее при удалении контроллера из списка зарегистрированных устройств
	/// </summary>
	public class ControllerDeletedEvent : CompositePresentationEvent<Guid>
	{
	}
}