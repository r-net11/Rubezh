using System;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	/// <summary>
	/// Описывает событие, происходящее при удалении фильтра журнала событий из списка зарегистрированных
	/// </summary>
	public class FilterDeletedEvent : CompositePresentationEvent<Guid>
	{
	}
}