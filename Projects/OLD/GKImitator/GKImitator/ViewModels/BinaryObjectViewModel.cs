using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using XFiresecAPI;
using Infrastructure.Common;
using GKImitator.Processor;
using System.Collections.ObjectModel;
using GKProcessor;

namespace GKImitator.ViewModels
{
	public class BinaryObjectViewModel : BaseViewModel
	{
		public BinaryObjectViewModel(BaseDescriptor binaryObject)
		{
			SetAutomaticRegimeCommand = new RelayCommand(OnSetAutomaticRegime);
			SetManualRegimeCommand = new RelayCommand(OnSetManualRegime);
			SetIgnoreRegimeCommand = new RelayCommand(OnSetIgnoreRegime);

			BinaryObject = binaryObject;
			Description = binaryObject.XBase.PresentationName;
			switch (binaryObject.DescriptorType)
			{
				case DescriptorType.Device:
					ImageSource = binaryObject.Device.Driver.ImageSource;
					break;

				case DescriptorType.Zone:
				case DescriptorType.Direction:
					ImageSource = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System).ImageSource;
					break;
			}

			//Formula = BinaryObject.Formula.GetStringFomula();

			StateBits = new ObservableCollection<StateBitViewModel>();
			StateBits.Add(new StateBitViewModel(this, XStateBit.Norm, true));
			StateBits.Add(new StateBitViewModel(this, XStateBit.Fire1));
			StateBits.Add(new StateBitViewModel(this, XStateBit.Fire2));
			StateBits.Add(new StateBitViewModel(this, XStateBit.On));
		}

		public BaseDescriptor BinaryObject { get; set; }
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
			imitatorJournalItem.GkObjectNo = BinaryObject.GetDescriptorNo();
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