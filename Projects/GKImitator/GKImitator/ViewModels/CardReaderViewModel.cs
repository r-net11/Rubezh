using RubezhAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using RubezhDAL.DataClasses;

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

		int _cardNo;
		public int CardNo
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
			DescriptorViewModel.CurrentCardNo = CardNo;
			var backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += backgroundWorker_DoWork;
			backgroundWorker.RunWorkerAsync();
			switch (SelectedEnterType)
			{
				case GKCodeReaderEnterType.CodeOnly:
					DescriptorViewModel.SetStateBit(GKStateBit.Fire1, false);
					DescriptorViewModel.SetStateBit(GKStateBit.Fire2, false);
					DescriptorViewModel.SetStateBit(GKStateBit.Attention, true);
					break;

				case GKCodeReaderEnterType.CodeAndOne:
					DescriptorViewModel.SetStateBit(GKStateBit.Attention, false);
					DescriptorViewModel.SetStateBit(GKStateBit.Fire2, false);
					DescriptorViewModel.SetStateBit(GKStateBit.Fire1, true);
					break;

				case GKCodeReaderEnterType.CodeAndTwo:
					DescriptorViewModel.SetStateBit(GKStateBit.Attention, false);
					DescriptorViewModel.SetStateBit(GKStateBit.Fire1, false);
					DescriptorViewModel.SetStateBit(GKStateBit.Fire2, true);
					break;
			}
		}

		void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			Thread.Sleep(TimeSpan.FromSeconds(3));
			var journalItem = new ImitatorJournalItem(2, 14, 0, 0);
			DescriptorViewModel.SetStateBit(GKStateBit.Attention, false);
			DescriptorViewModel.SetStateBit(GKStateBit.Fire1, false);
			DescriptorViewModel.SetStateBit(GKStateBit.Fire2, false);
			DescriptorViewModel.AddJournalItem(journalItem);
			DescriptorViewModel.CurrentCardNo = 0;
		}
	}
}