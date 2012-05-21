using CallModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using System.Collections.Generic;
using Infrastructure.Common.Navigation;

namespace CallModule
{
    public class CallModuleLoader : ModuleBase
    {
        CallViewModel CallViewModel;

        public CallModuleLoader()
        {
            ServiceFactory.Events.GetEvent<ShowCallEvent>().Subscribe(OnShowCall);
        }

        void CreateViewModels()
        {
            CallViewModel = new CallViewModel();
        }

        void OnShowCall(object obj)
        {
            ServiceFactory.Layout.Show(CallViewModel);
        }

		public override void Initialize()
		{
			CreateViewModels();
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