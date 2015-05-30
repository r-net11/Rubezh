﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Journal;
using GKImitator.Processor;
using FiresecAPI.GK;

namespace GKImitator.ViewModels
{
	public class FailureViewModel : BaseViewModel
	{
		public FailureViewModel(BinaryObjectViewModel binaryObjectViewModel, JournalEventDescriptionType journalEventDescriptionType, byte no)
		{
			BinaryObjectViewModel = binaryObjectViewModel;
			JournalEventDescriptionType = journalEventDescriptionType;
			No = no;
		}

		BinaryObjectViewModel BinaryObjectViewModel;
		public JournalEventDescriptionType JournalEventDescriptionType { get; private set; }
		public byte No { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);

				var journalItem = new ImitatorJournalItem();

				journalItem.Source = 2;
				journalItem.Code = 5;
				journalItem.EventDescription = No;
				journalItem.EventYesNo = (byte)(value ? 1 : 0);

				journalItem.ObjectNo = 0;
				journalItem.ObjectDeviceType = 0;
				journalItem.ObjectDeviceAddress = 0;
				journalItem.ObjectFactoryNo = 0;
				journalItem.ObjectState = 0;

				journalItem.ObjectDeviceType = (short)(BinaryObjectViewModel.BinaryObject.GKBase as GKDevice).Driver.DriverTypeNo;
				journalItem.ObjectDeviceAddress = (short)(((BinaryObjectViewModel.BinaryObject.GKBase as GKDevice).ShleifNo - 1) * 256 + (BinaryObjectViewModel.BinaryObject.GKBase as GKDevice).IntAddress);

				BinaryObjectViewModel.AddJournalItem(journalItem);
			}
		}
	}
}