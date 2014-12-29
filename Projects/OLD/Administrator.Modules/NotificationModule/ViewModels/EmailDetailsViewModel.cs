using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace NotificationModule.ViewModels
{
	public class EmailDetailsViewModel : SaveCancelDialogViewModel
	{
		public EmailViewModel EmailViewModel { get; private set; }

		public EmailDetailsViewModel()
		{
			SelectZonesCommand = new RelayCommand(OnSelectZonesCommand);
			StateTypes = new ObservableCollection<StateTypeViewModel>();
			foreach (StateType stateType in Enum.GetValues(typeof(StateType)))
			{
				StateTypes.Add(new StateTypeViewModel(stateType));
			}
			Title = "Создать получателя";
			EmailViewModel = new EmailViewModel();
		}

		public EmailDetailsViewModel(Email email)
		{
			SelectZonesCommand = new RelayCommand(OnSelectZonesCommand);
			StateTypes = new ObservableCollection<StateTypeViewModel>();
			foreach (StateType stateType in Enum.GetValues(typeof(StateType)))
			{
				StateTypes.Add(new StateTypeViewModel(stateType));
			}
			Title = "Редактировать получателя";
			EmailViewModel = new EmailViewModel(email);
			StateTypes.Where(
				eventViewModel => email.States.Any(
					x => x == eventViewModel.StateType)).All(x => x.IsChecked = true);
		}

		public ObservableCollection<StateTypeViewModel> StateTypes { get; private set; }

		StateType _selectedStateType;

		public StateType SelectedStateType
		{
			get { return _selectedStateType; }
			set
			{
				_selectedStateType = value;
				OnPropertyChanged(() => SelectedStateType);
			}
		}

		protected override bool Save()
		{
			EmailViewModel.Email.States = StateTypes.Where(x => x.IsChecked).Select(x => x.StateType).Cast<StateType>().ToList();
			return base.Save();
		}

		protected override bool CanSave()
		{
			return IsValidEmail(EmailViewModel.Email.Address);
		}

		public static bool IsValidEmail(string address)
		{
			return
				address != null &&
				Regex.IsMatch(address,
					@"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
					@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
		}

		public RelayCommand SelectZonesCommand { get; private set; }

		private void OnSelectZonesCommand()
		{
			if (EmailViewModel.Email.Zones == null)
				EmailViewModel.Email.Zones = new List<Guid>();
			var zoneSelectionViewModel = new ZoneSelectionViewModel(EmailViewModel.Email.Zones);
			if (DialogService.ShowModalWindow(zoneSelectionViewModel))
			{
				EmailViewModel.Email.Zones = zoneSelectionViewModel.ChosenZonesList;
				EmailViewModel.Update();
			}
		}
	}
}