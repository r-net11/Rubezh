using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Mail;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace NotificationModule.ViewModels
{
	public class NotificationViewModel : MenuViewPartViewModel, IEditingViewModel
	{
		public NotificationViewModel()
		{
			Menu = new NotificationMenuViewModel(this);
			TestCommand = new RelayCommand(OnTest, CanEditDelete);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			RegisterShortcuts();
		}

		public void Initialize()
		{
			Emails = new ObservableCollection<EmailViewModel>();

			if (FiresecManager.SystemConfiguration.Emails == null)
			{
				FiresecManager.SystemConfiguration.Emails = new List<Email>();
				AddSampleEmail();
			}

			foreach (var email in FiresecManager.SystemConfiguration.Emails)
			{
				var emailViewModel = new EmailViewModel(email);
				Emails.Add(emailViewModel);
			}

			SelectedEmail = Emails.FirstOrDefault();
		}

		ObservableCollection<EmailViewModel> _emails;
		public ObservableCollection<EmailViewModel> Emails
		{
			get { return _emails; }
			set
			{
				_emails = value;
				OnPropertyChanged("Emails");
			}
		}

		EmailViewModel _selectedEmail;

		public EmailViewModel SelectedEmail
		{
			get { return _selectedEmail; }
			set
			{
				_selectedEmail = value;
				OnPropertyChanged("SelectedEmail");
			}
		}

		private static void AddSampleEmail()
		{
			FiresecManager.SystemConfiguration.Emails.Add(new Email
			{
				Address = "obychevma@rubezh.ru",
				FirstName = "Максим",
				LastName = "Обычев",
				SendingStates = new List<StateType>()
			});
			foreach (Email email in FiresecManager.SystemConfiguration.Emails)
			{
				email.SendingStates.Add(StateType.Fire);
				email.SendingStates.Add(StateType.Failure);
			}
		}

		public RelayCommand TestCommand { get; private set; }

		private void OnTest()
		{
			MailHelper.Send(SelectedEmail.Email.Address,
							"Этот адресат уведомляется о следующих состояниях: " + SelectedEmail.PresenrationStates,
							"Тестовое собщение Firesec-2");
		}

		public RelayCommand AddCommand { get; private set; }

		private void OnAdd()
		{
			var emailDetailsViewModel = new EmailDetailsViewModel();
			if (DialogService.ShowModalWindow(emailDetailsViewModel))
			{
				FiresecManager.SystemConfiguration.Emails.Add(emailDetailsViewModel.Email);
				var emailViewModel = new EmailViewModel(emailDetailsViewModel.Email);
				Emails.Add(emailViewModel);
				SelectedEmail = emailViewModel;
				ServiceFactory.SaveService.EmailsChanged = true;
			}
		}

		private bool CanEditDelete()
		{
			return SelectedEmail != null;
		}

		public RelayCommand DeleteCommand { get; private set; }

		private void OnDelete()
		{
			FiresecManager.SystemConfiguration.Emails.Remove(SelectedEmail.Email);
			Emails.Remove(SelectedEmail);
			ServiceFactory.SaveService.EmailsChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }

		private void OnEdit()
		{
			var emailDetailsViewModel = new EmailDetailsViewModel(SelectedEmail.Email);
			if (DialogService.ShowModalWindow(emailDetailsViewModel))
			{
				SelectedEmail.Email = emailDetailsViewModel.Email;
				ServiceFactory.SaveService.CamerasChanged = true;
			}
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}
	}
}