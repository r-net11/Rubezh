using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class MPTDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKMPT MPT { get; set; }
		public ObservableCollection<string> AvailableNames { get; private set; }
		public ObservableCollection<string> AvailableDescription { get; private set; }

		public MPTDetailsViewModel(GKMPT mpt = null)
		{
			if (mpt == null)
			{
				Title = "Создание нового МПТ";

				MPT = new GKMPT()
				{
					Name = "Новый МПТ",
					No = 1,
				};
				if (GKManager.MPTs.Count != 0)
					MPT.No = (GKManager.MPTs.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства МПТ: {0}", mpt.Name);
				MPT = mpt;
			}
			CopyProperties();
			var availableNames = new HashSet<string>();
			var availableDescription = new HashSet<string>();
			foreach (var existingMPTs in GKManager.MPTs)
			{
				availableNames.Add(existingMPTs.Name);
				availableDescription.Add(existingMPTs.Description);
			}
			AvailableNames = new ObservableCollection<string>(availableNames);
			AvailableDescription = new ObservableCollection<string>(availableDescription);
		}

		void CopyProperties()
		{
			No = MPT.No;
			Name = MPT.Name;
			Description = MPT.Description;
		}

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

		protected override bool Save()
		{
			if (No <= 0)
			{
				MessageBoxService.Show("Номер должен быть положительным числом");
				return false;
			}
			if (MPT.No != No && GKManager.MPTs.Any(x => x.No == No))
			{
				MessageBoxService.Show("МПТ с таким номером уже существует");
				return false;
			}

			MPT.No = No;
			MPT.Name = Name;
			MPT.Description = Description;
			return base.Save();
		}
	}
}