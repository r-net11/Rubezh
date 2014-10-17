using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure.Events;
using Infrastructure;
using FiresecAPI.Journal;
using System.Collections.Generic;
using System.ComponentModel;
using FiresecClient;
using FiresecAPI.Models;
using FiresecAPI;

namespace FireMonitor.ViewModels
{
	public class AutoActivationViewModel : BaseViewModel
	{
		public AutoActivationViewModel()
		{
			ChangeAutoActivationCommand = new RelayCommand(OnChangeAutoActivation);
			ChangePlansAutoActivationCommand = new RelayCommand(OnChangePlansAutoActivation);

			ServiceFactory.Events.GetEvent<UserChangedEvent>().Unsubscribe(OnUserChanged);
			ServiceFactory.Events.GetEvent<UserChangedEvent>().Subscribe(OnUserChanged);
		}

		public bool HasPermission
		{
			get { return FiresecManager.CheckPermission(PermissionType.Oper_ChangeView); }
		}
		public bool IsAutoActivation
		{
			get { return ClientSettings.AutoActivationSettings.IsAutoActivation; }
			set
			{
				ClientSettings.AutoActivationSettings.IsAutoActivation = value;
				OnPropertyChanged(() => IsAutoActivation);
			}
		}
		public bool IsPlansAutoActivation
		{
			get { return ClientSettings.AutoActivationSettings.IsPlansAutoActivation; }
			set
			{
				ClientSettings.AutoActivationSettings.IsPlansAutoActivation = value;
				OnPropertyChanged(() => IsPlansAutoActivation);
			}
		}

		public RelayCommand ChangeAutoActivationCommand { get; private set; }
		void OnChangeAutoActivation()
		{
			IsAutoActivation = !IsAutoActivation;
		}

		public RelayCommand ChangePlansAutoActivationCommand { get; private set; }
		void OnChangePlansAutoActivation()
		{
			IsPlansAutoActivation = !IsPlansAutoActivation;
			OnPropertyChanged(() => IsPlansAutoActivation);
		}

        private void OnUserChanged(UserChangedEventArgs userChangedEventArgs)
        {
            OnPropertyChanged(() => HasPermission);
        }
 	}
}