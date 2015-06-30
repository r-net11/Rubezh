﻿using Infrastructure.Common.Services.Content;
using Infrastructure.Common.Services.DragDrop;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Common.Services
{
	public abstract class ServiceFactoryBase
	{
		public static IEventAggregator Events { get; set; }
		public static IResourceService ResourceService { get; protected set; }
		public static IContentService ContentService { get; protected set; }
		public static IDragDropService DragDropService { get; protected set; }
		public static ISecurityService SecurityService { get; protected set; }

		public static void OnPublishEvent<T, TEvent>(T item)
			where TEvent : CompositePresentationEvent<T>, new()
		{
			ServiceFactoryBase.Events.GetEvent<TEvent>().Publish(item);
		}
		public static void OnPublishEvent<T, TEvent>()
			where TEvent : CompositePresentationEvent<T>, new()
		{
			OnPublishEvent<T,TEvent>(default(T));
		}
	}
}