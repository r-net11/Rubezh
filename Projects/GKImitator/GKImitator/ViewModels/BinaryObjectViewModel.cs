using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Common.GK;
using FiresecClient;
using XFiresecAPI;
using Infrastructure.Common;
using GKImitator.Processor;
using System.Collections.ObjectModel;

namespace GKImitator.ViewModels
{
	public class BinaryObjectViewModel : BaseViewModel
	{
		public BinaryObjectViewModel(BinaryObjectBase binaryObject)
		{
			SetAutomaticRegimeCommand = new RelayCommand(OnSetAutomaticRegime);
			SetManualRegimeCommand = new RelayCommand(OnSetManualRegime);
			SetIgnoreRegimeCommand = new RelayCommand(OnSetIgnoreRegime);

			BinaryObject = binaryObject;
			Description = binaryObject.BinaryBase.GetBinaryDescription();
			switch (binaryObject.ObjectType)
			{
				case ObjectType.Device:
					ImageSource = binaryObject.Device.Driver.ImageSource;
					break;

				case ObjectType.Zone:
				case ObjectType.Direction:
					ImageSource = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System).ImageSource;
					break;
			}

			Formula = BinaryObject.Formula.GetStringFomula();

			StateBits = new ObservableCollection<StateBitViewModel>();
			StateBits.Add(new StateBitViewModel(this, XStateBit.Norm, true));
			StateBits.Add(new StateBitViewModel(this, XStateBit.Fire1));
			StateBits.Add(new StateBitViewModel(this, XStateBit.Fire2));
		}

		public BinaryObjectBase BinaryObject { get; set; }
		public string ImageSource { get; set; }
		public string Description { get; set; }
		public string Formula { get; set; }

		public ObservableCollection<StateBitViewModel> StateBits { get; private set; }

		public RelayCommand SetAutomaticRegimeCommand { get; private set; }
		void OnSetAutomaticRegime()
		{

		}

		public RelayCommand SetManualRegimeCommand { get; private set; }
		void OnSetManualRegime()
		{

		}

		public RelayCommand SetIgnoreRegimeCommand { get; private set; }
		void OnSetIgnoreRegime()
		{
			var imitatorJournalItem = new ImitatorJournalItem();
			imitatorJournalItem.DateTime = DateTime.Now;
			imitatorJournalItem.GkNo = JournalHelper.ImitatorJournalItemCollection.ImitatorJournalItems.Count + 1;
			imitatorJournalItem.GkObjectNo = BinaryObject.GkDescriptorNo;
			imitatorJournalItem.UNUSED_KauNo = 0;
			imitatorJournalItem.UNUSED_KauAddress = 0;

			imitatorJournalItem.Source = 2; // Controller = 0, Device = 1, Object = 2
			imitatorJournalItem.Code = 10;
			imitatorJournalItem.EventDescription = 2;
			imitatorJournalItem.EventYesNo = 0;

			imitatorJournalItem.ObjectNo = 0;
			imitatorJournalItem.ObjectDeviceType = 0;
			imitatorJournalItem.ObjectDeviceAddress = 0;
			imitatorJournalItem.ObjectFactoryNo = 0;
			imitatorJournalItem.ObjectState = 0;

			if (BinaryObject.Device != null)
			{
				imitatorJournalItem.ObjectDeviceType = (short)BinaryObject.Device.Driver.DriverTypeNo;
				imitatorJournalItem.ObjectDeviceAddress = (short)((BinaryObject.Device.ShleifNo - 1) * 256 + BinaryObject.Device.IntAddress);
			}

			JournalHelper.ImitatorJournalItemCollection.ImitatorJournalItems.Add(imitatorJournalItem);
			JournalHelper.Save();
		}
	}
}