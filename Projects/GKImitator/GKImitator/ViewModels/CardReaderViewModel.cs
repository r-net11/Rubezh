﻿using RubezhAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhDAL.DataClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

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
			switch (SelectedEnterType)
			{
				case GKCodeReaderEnterType.CodeOnly:
					DescriptorViewModel.SetStateBit(GKStateBit.Attention, true);
					DescriptorViewModel.SetStateBit(GKStateBit.Fire1, false);
					DescriptorViewModel.SetStateBit(GKStateBit.Fire2, false);
					var journalItem = new ImitatorJournalItem(2, 4, 0, 0);
					DescriptorViewModel.AddJournalItem(journalItem);
					break;

				case GKCodeReaderEnterType.CodeAndOne:
					DescriptorViewModel.SetStateBit(GKStateBit.Attention, false);
					DescriptorViewModel.SetStateBit(GKStateBit.Fire1, true);
					DescriptorViewModel.SetStateBit(GKStateBit.Fire2, false);
					journalItem = new ImitatorJournalItem(2, 2, 0, 0);
					DescriptorViewModel.AddJournalItem(journalItem);
					break;

				case GKCodeReaderEnterType.CodeAndTwo:
					DescriptorViewModel.SetStateBit(GKStateBit.Attention, false);
					DescriptorViewModel.SetStateBit(GKStateBit.Fire1, false);
					DescriptorViewModel.SetStateBit(GKStateBit.Fire2, true);
					journalItem = new ImitatorJournalItem(2, 3, 0, 0);
					DescriptorViewModel.AddJournalItem(journalItem);
					break;
			}
			DescriptorViewModel.CurrentCardNo = CardNo;
			DescriptorViewModel.RecalculateOutputLogic();

			var backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += backgroundWorker_DoWork;
			backgroundWorker.RunWorkerAsync();
		}

		void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			Thread.Sleep(TimeSpan.FromSeconds(3));
			DescriptorViewModel.SetStateBit(GKStateBit.Attention, false);
			DescriptorViewModel.SetStateBit(GKStateBit.Fire1, false);
			DescriptorViewModel.SetStateBit(GKStateBit.Fire2, false);
			var journalItem = new ImitatorJournalItem(2, 14, 0, 0);
			DescriptorViewModel.AddJournalItem(journalItem);
			DescriptorViewModel.CurrentCardNo = 0;
			DescriptorViewModel.RecalculateOutputLogic();
		}
	}
}