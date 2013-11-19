using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using Infrastructure.Events;
using Infrastructure.Common.Windows;
using System.ComponentModel;

namespace GKModule
{
	public static class UsersWatchManager
	{
		public static void Start()
		{
			ServiceFactory.Events.GetEvent<UserChangedEvent>().Unsubscribe(OnUserChanged);
			ServiceFactory.Events.GetEvent<UserChangedEvent>().Subscribe(OnUserChanged);

			ApplicationService.Closed -= new EventHandler(ApplicationService_Closed);
			ApplicationService.Closed += new EventHandler(ApplicationService_Closed);
		}

		public static void OnUserChanged(UserChangedEventArgs userChangedEventArgs)
		{
			if (userChangedEventArgs.IsReconnect)
			{
				JournaActionlHelper.Add("Смена пользователя", userChangedEventArgs.OldName + " вышел. " + userChangedEventArgs.NewName + " вошел");
			}
			else
			{
				JournaActionlHelper.Add("Вход пользователя в систему", "");
			}
		}

		static void ApplicationService_Closed(object sender, EventArgs e)
		{
			JournaActionlHelper.Add("Выход пользователя из системы", "");
		}
	}
}