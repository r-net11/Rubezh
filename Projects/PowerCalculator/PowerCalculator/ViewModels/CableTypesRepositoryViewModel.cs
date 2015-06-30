using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;
using PowerCalculator.Processor;
using System.Collections.Generic;
using System;
using Infrastructure.Common;

namespace PowerCalculator.ViewModels
{
	public class CableTypesRepositoryViewModel : SaveCancelDialogViewModel
	{
        public CableTypesRepositoryViewModel()
		{
			Title = "Редактирование списка кабелей";
            CableTypes = new ObservableCollection<CableTypeViewModel>(CableTypesRepository.CableTypes.Where(x => x != CableTypesRepository.CustomCableType).Select(x=>new CableTypeViewModel(x)));
            SelectedCableType = CableTypes.FirstOrDefault();
            AddCommand = new RelayCommand(OnAdd);
            RemoveCommand = new RelayCommand(OnRemove, CanRemove);
		}

		public ObservableCollection<CableTypeViewModel> CableTypes { get; private set; }
        
        CableTypeViewModel _selectedCableType;
        public CableTypeViewModel SelectedCableType
        {
            get { return _selectedCableType; }
            set
            {
                _selectedCableType = value;
                OnPropertyChanged(() => SelectedCableType);
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            CableTypes.Add(new CableTypeViewModel(new CableType() { Name = "Название кабеля", Resistivity = 0.05 }));
            SelectedCableType = CableTypes.LastOrDefault();
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            var index = CableTypes.IndexOf(SelectedCableType);
            CableTypes.Remove(SelectedCableType);
            if (index == CableTypes.Count)
                index--;
            if (index != -1)
                SelectedCableType = CableTypes[index];
        }
        bool CanRemove()
        {
            return SelectedCableType != null;
        }

        protected override bool Save()
        {

            CableTypesRepository.Initialize();
            CableTypesRepository.CableTypes.AddRange(CableTypes.Select(x => x.CableType));
            return base.Save();
        }
	}
}