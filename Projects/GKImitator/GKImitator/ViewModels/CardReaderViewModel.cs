using FiresecAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
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
			SelectedEnterType = EnterTypes.FirstOrDefault();
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
			DescriptorViewModel.StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Attention).IsActive = true;
			DescriptorViewModel.CurrentCardNo = CardNo;
			var backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += backgroundWorker_DoWork;
			backgroundWorker.RunWorkerAsync();
		}

		void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			Thread.Sleep(TimeSpan.FromSeconds(3));
			DescriptorViewModel.StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Attention).IsActive = false;
			DescriptorViewModel.CurrentCardNo = 0;
		}
	}
}