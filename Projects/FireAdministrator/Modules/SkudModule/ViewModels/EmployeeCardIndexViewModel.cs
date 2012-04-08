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
		public RelayCommand DeleteAllCommand { get; private set; }
		private EmployeeCardViewModel _selectedCard;

		public EmployeeCardIndexViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditRemove);
			DeleteAllCommand = new RelayCommand(OnDeleteAll, CanRemoveAll);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);
			EmployeeCardIndex = new ObservableCollection<EmployeeCardViewModel>();
		}

		public void Initialize()
		{
			// loading...
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
			//FiresecManager.SystemConfiguration.Cards.Remove(SelectedCard.Instruction);
			EmployeeCardIndex.Remove(SelectedEmployeeCard);
			if (EmployeeCardIndex.IsNotNullOrEmpty())
				SelectedEmployeeCard = EmployeeCardIndex[0];
			//ServiceFactory.SaveService.InstructionsChanged = true;
		}
		void OnDeleteAll()
		{
			SelectedEmployeeCard = null;
			EmployeeCardIndex.Clear();
			//FiresecManager.SystemConfiguration.Cards.Clear();
			//ServiceFactory.SaveService.InstructionsChanged = true;
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