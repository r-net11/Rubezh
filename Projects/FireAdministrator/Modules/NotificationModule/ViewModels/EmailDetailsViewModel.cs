using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace NotificationModule.ViewModels
{
	public class EmailDetailsViewModel : SaveCancelDialogViewModel
	{
		public Email Email { get; private set; }

		public EmailDetailsViewModel()
		{
			StateTypes = new ObservableCollection<StateTypeViewModel>();
			foreach (StateType stateType in Enum.GetValues(typeof(StateType)))
			{
				StateTypes.Add(new StateTypeViewModel(stateType));
			}
			Title = "Создать получателя";
			Email = new Email();
		}

		public EmailDetailsViewModel(Email email)
		{
			StateTypes = new ObservableCollection<StateTypeViewModel>();
			foreach (StateType stateType in Enum.GetValues(typeof(StateType)))
			{
				StateTypes.Add(new StateTypeViewModel(stateType));
			}
			Title = "Редактировать получателя";
			Email = email;
			StateTypes.Where(
				eventViewModel => email.SendingStates.Any(
					x => x == eventViewModel.StateType)).All(x => x.IsChecked = true);
		}

		string _firstName;

		public string FirstName
		{
			get { return _firstName; }
			set
			{
				_firstName = value;
			}
		}

		string _LastName;

		public string LastName
		{
			get { return _LastName; }
			set
			{
				_LastName = value;
			}
		}

		string _address;

		public string Address
		{
			get { return _address; }
			set
			{
				_address = value;
			}
		}

		public ObservableCollection<StateTypeViewModel> StateTypes { get; private set; }

		StateType _selectedStateType;

		public StateType SelectedStateType
		{
			get { return _selectedStateType; }
			set
			{
				_selectedStateType = value;
				OnPropertyChanged("SelectedStateType");
			}
		}

		protected override bool Save()
		{
			Email.SendingStates = StateTypes.Where(x => x.IsChecked).Select(x => x.StateType).Cast<StateType>().ToList();
			return base.Save();
		}

		protected override bool CanSave()
		{
			return IsValidEmail(Email.Address);
		}

		public static bool IsValidEmail(string strIn)
		{
			return
				strIn != null &&
				Regex.IsMatch(strIn,
					@"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
					@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
		}
	}
}