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

			ApplicationService.Closing -= new System.ComponentModel.CancelEventHandler(ApplicationService_Closing);
			ApplicationService.Closing += new System.ComponentModel.CancelEventHandler(ApplicationService_Closing);
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

		static void ApplicationService_Closing(object sender, CancelEventArgs e)
		{
			JournaActionlHelper.Add("Выход пользователя из системы", "");
		}
	}
}