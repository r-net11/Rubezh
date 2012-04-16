using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;

namespace SkudModule.ViewModels
{
	public class EmployeeCardIndexViewModel : RegionViewModel, IEditingViewModel
	{
		public ObservableCollection<EmployeeCardViewModel> EmployeeCardIndex { get; set; }
		public RelayCommand AddCommand { get; private set; }
		public RelayCommand EditCommand { get; private set; }
		public RelayCommand DeleteCommand { get; private set; }
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
		void OnEdit()
		{
			//var instructionDetailsViewModel = new InstructionDetailsViewModel(SelectedInstruction.Instruction);
			//if (ServiceFactory.UserDialogs.ShowModalWindow(instructionDetailsViewModel))
			//{
			//    SelectedInstruction.Update();
			//    ServiceFactory.SaveService.InstructionsChanged = true;
			//}
		}
		void OnDelete()
		{
			FiresecManager.sk.Cards.Remove(SelectedEmployeeCard.EmployeeCard);
			EmployeeCardIndex.Remove(SelectedEmployeeCard);
			if (EmployeeCardIndex.IsNotNullOrEmpty())
				SelectedEmployeeCard = EmployeeCardIndex[0];
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