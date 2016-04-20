using RubezhAPI.GK;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GKImitator.ViewModels
{
	public class CardReaderViewModel : DialogViewModel
	{
		DescriptorViewModel DescriptorViewModel { get; set; }
		public CardReaderViewModel(DescriptorViewModel descriptorViewModel)
		{
			Title = "Считывание карт";
			EnterCommand = new RelayCommand(OnEnter);
			DescriptorViewModel = descriptorViewModel;

			EnterTypes = Enum.GetValues(typeof(GKCodeReaderEnterType)).Cast<GKCodeReaderEnterType>().ToList();
			SelectedEnterType = EnterTypes.FirstOrDefault(x => x == GKCodeReaderEnterType.CodeOnly);
		}

		public List<GKCodeReaderEnterType> EnterTypes { get; private set; }

		GKCodeReaderEnterType _selectedEnterType;
		public GKCodeReaderEnterType SelectedEnterType
		{
			get { return _selectedEnterType; }
			set
			{
				_selectedEnterType = value;
				OnPropertyChanged(() => SelectedEnterType);
			}
		}

		uint _cardNo;
		public uint CardNo
		{
			get { return _cardNo; }
			set
			{
				_cardNo = value;
				OnPropertyChanged(() => CardNo);
			}
		}

		public RelayCommand EnterCommand { get; private set; }
		void OnEnter()
		{
			DescriptorViewModel.EnterCard(CardNo, SelectedEnterType);
		}
	}
}