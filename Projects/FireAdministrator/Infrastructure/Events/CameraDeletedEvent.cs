using System;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	/// <summary>
	/// Описывает событие, происходящее при удалении камеры из списка зарегистрированных камер в модуле 'Видео'
	/// </summary>
	public class CameraDeletedEvent : CompositePresentationEvent<Guid>
	{
	}
}