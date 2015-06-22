using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DoorDetailsViewModel : SaveCancelDialogViewModel
	{

		int _no;
		public int No
		{
			get { return _no; }
			set
			{
				_no = value;
				OnPropertyChanged(() => No);
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

	
		int _delay;
		public int Delay
		{
			get { return _delay; }
			set
			{
				_delay = value;
				OnPropertyChanged(() => Delay);
			}
		}

		int _hold;
		public int Hold
		{
			get { return _hold; }
			set
			{
				_hold = value;
				OnPropertyChanged(() => Hold);
			}
		}

		int _enterLevel;
		public int EnterLevel
		{
			get { return _enterLevel; }
			set
			{
				_enterLevel = value;
				OnPropertyChanged(() => EnterLevel);
			}
		}

		bool _antipassbackOn;
		public bool AntipassbackOn
		{
			get { return _antipassbackOn; }
			set
			{
				_antipassbackOn = value;
				OnPropertyChanged(() => AntipassbackOn);
			}
		}

		public ObservableCollection<string> AvailableNames { get; private set; }
		public ObservableCollection<string> AvailableDescription { get; private set; }

		protected override bool Save()
		{
			if (No <= 0)
			{
				MessageBoxService.Show("Номер должен быть положительным числом");
				return false;
			}

			return base.Save();
		}
	}
}