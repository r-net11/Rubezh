using System.Collections.Generic;
using CallModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;

namespace CallModule
{
    public class CallModuleLoader : ModuleBase
    {
        CallViewModel CallViewModel;

        public CallModuleLoader()
        {
            ServiceFactory.Events.GetEvent<ShowCallEvent>().Subscribe(OnShowCall);
			CallViewModel = new CallViewModel();
        }

        void OnShowCall(object obj)
        {
            ServiceFactory.Layout.Show(CallViewModel);
        }

		public override void Initialize()
		{
			CallViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				//new NavigationItem<ShowCallEvent>("Дозвон", "/Controls;component/Images/phone.png"),
			};
		}
	}
}