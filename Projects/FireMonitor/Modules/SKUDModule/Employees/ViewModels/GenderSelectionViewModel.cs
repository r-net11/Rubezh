using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SelectGenderViewModel : SaveCancelDialogViewModel
	{
		public SelectGenderViewModel(Gender employeeGender)
		{
			Genders = new List<SelectationGenderViewModel>();
		  	var genders = Enum.GetValues(typeof(Gender));
			foreach (Gender gender in genders)
				Genders.Add(new SelectationGenderViewModel(gender));
			SelectedGender = Genders.FirstOrDefault(x => x.Gender == employeeGender);
			SelectedGender.IsChecked = true;
		}

		public List<SelectationGenderViewModel> Genders { get; private set; }

		SelectationGenderViewModel _selectedGender;
		public SelectationGenderViewModel SelectedGender
		{
			get { return _selectedGender; }
			set
			{
				_selectedGender = value;
				OnPropertyChanged(() => SelectedGender);
			}
		}

		protected override bool Save()
		{
			SelectedGender = Genders.FirstOrDefault(x => x.IsChecked);
			return base.Save();
		}
	}

	public class SelectationGenderViewModel : BaseViewModel
	{
		public Gender Gender { get; private set; }

		public SelectationGenderViewModel(Gender gender)
		{
			Gender = gender;
		}

		public string Name { get { return Gender.ToDescription(); } }
		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}
	}
}
