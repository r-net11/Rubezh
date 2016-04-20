using System.Collections.Generic;
using FiresecAPI;
using Infrastructure.Client;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Navigation;
using Infrastructure.Events;
using NotificationModule.ViewModels;

namespace NotificationModule
{
	public class NotificationModule : ModuleBase
	{
		NotificationViewModel NotificationViewModel;

		public override void CreateViewModels()
		{
			NotificationViewModel = new NotificationViewModel();
		}

		public override void Initialize()
		{
			NotificationViewModel.Initialize();
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowNotificationEvent>(NotificationViewModel, ModuleType.ToDescription(), "music"),
			};
		}

		public override ModuleType ModuleType
		{
			get { return ModuleType.Notification; }
		}
	}
}