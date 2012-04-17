using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;
using FiresecAPI.Models.Skud;

namespace SkudModule.ViewModels
{
	public class EmployeeCardIndexViewModel : RegionViewModel, IEditingViewModel
	{
		public ObservableCollection<EmployeeCardViewModel> EmployeeCardIndex { get; set; }
		private EmployeeCardViewModel _selectedCard;

		public EmployeeCardIndexViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditRemove);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);
			EmployeeCardIndex = new ObservableCollection<EmployeeCardViewModel>();
		}

		public void Initialize()
		{
			var list = FiresecManager.GetEmployees();
			if (list != null)
				foreach (var employee in list)
					EmployeeCardIndex.Add(new EmployeeCardViewModel(employee));

			if (EmployeeCardIndex.Count > 0)
				SelectedEmployeeCard = EmployeeCardIndex[0];
		}

		public override void OnShow()
		{
			var сardIndexMenuViewModel = new EmployeeCardIndexMenuViewModel(this);
			ServiceFactory.Layout.ShowMenu(сardIndexMenuViewModel);
		}
		public override void OnHide()
		{
			ServiceFactory.Layout.ShowMenu(null);
		}


		public EmployeeCardViewModel SelectedEmployeeCard
		{
			get { return _selectedCard; }
			set
			{
				_selectedCard = value;
				OnPropertyChanged("SelectedEmployeeCard");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			//var instructionDetailsViewModel = new InstructionDetailsViewModel();
			//if (ServiceFactory.UserDialogs.ShowModalWindow(instructionDetailsViewModel))
			//{
			//    Instructions.Add(new InstructionViewModel(instructionDetailsViewModel.Instruction));
			//    FiresecManager.SystemConfiguration.Instructions.Add(instructionDetailsViewModel.Instruction);
			//    ServiceFactory.SaveService.InstructionsChanged = true;
			//}
		}
		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			//var instructionDetailsViewModel = new InstructionDetailsViewModel(SelectedInstruction.Instruction);
			//if (ServiceFactory.UserDialogs.ShowModalWindow(instructionDetailsViewModel))
			//{
			//    SelectedInstruction.Update();
			//    ServiceFactory.SaveService.InstructionsChanged = true;
			//}
		}
		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (true) //add confirmation
			{
				ActionResult result = FiresecManager.EmployeeCardDelete(SelectedEmployeeCard.EmployeeCard);
				if (result.RowCount == 1)
				{
					int index = EmployeeCardIndex.IndexOf(SelectedEmployeeCard);
					EmployeeCardIndex.Remove(SelectedEmployeeCard);
					if (EmployeeCardIndex.IsNotNullOrEmpty())
						SelectedEmployeeCard = index < EmployeeCardIndex.Count ? EmployeeCardIndex[index] : EmployeeCardIndex[EmployeeCardIndex.Count - 1];
				}
				//else remove failed messageBox
			}
		}

		bool CanEditRemove()
		{
			return SelectedEmployeeCard != null;
		}
		bool CanRemoveAll()
		{
			return (EmployeeCardIndex.IsNotNullOrEmpty());
		}
	}
}