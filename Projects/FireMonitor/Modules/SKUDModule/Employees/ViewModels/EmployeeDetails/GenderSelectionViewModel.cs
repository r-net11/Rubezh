using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class GenderSelectionViewModel : SaveCancelDialogViewModel
	{
		public GenderSelectionViewModel(Gender employeeGender)
		{
			Genders = new List<SelectationGenderViewModel>();
			var genders = Enum.GetValues(typeof(Gender));
			foreach (Gender gender in genders)
				Genders.Add(new SelectationGenderViewModel(gender));
			var selectedGender = Genders.FirstOrDefault(x => x.Gender == employeeGender);
			selectedGender.IsChecked = true;
		}

		public List<SelectationGenderViewModel> Genders { get; private set; }
		public Gender Gender;

		protected override bool Save()
		{
			Gender = Genders.FirstOrDefault(x => x.IsChecked).Gender;
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