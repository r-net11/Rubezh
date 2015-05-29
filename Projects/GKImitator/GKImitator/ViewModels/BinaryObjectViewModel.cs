using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using GKImitator.Processor;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

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
			Description = binaryObject.GKBase.PresentationName;
			switch (binaryObject.DescriptorType)
			{
				case DescriptorType.Device:
					ImageSource = (binaryObject.GKBase as GKDevice).Driver.ImageSource;
					break;

				case DescriptorType.Zone:
				case DescriptorType.Direction:
					ImageSource = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.System).ImageSource;
					break;
			}

			//Formula = BinaryObject.Formula.GetStringFomula();

			StateBits = new ObservableCollection<StateBitViewModel>();
			StateBits.Add(new StateBitViewModel(this, GKStateBit.Norm, true));
			StateBits.Add(new StateBitViewModel(this, GKStateBit.Fire1));
			StateBits.Add(new StateBitViewModel(this, GKStateBit.Fire2));
			StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
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

			if (BinaryObject.GKBase != null && BinaryObject.GKBase is GKDevice)
			{
				imitatorJournalItem.ObjectDeviceType = (short)(BinaryObject.GKBase as GKDevice).Driver.DriverTypeNo;
				imitatorJournalItem.ObjectDeviceAddress = (short)(((BinaryObject.GKBase as GKDevice).ShleifNo - 1) * 256 + (BinaryObject.GKBase as GKDevice).IntAddress);
			}

			JournalHelper.ImitatorJournalItemCollection.ImitatorJournalItems.Add(imitatorJournalItem);
			JournalHelper.Save();
		}
	}
}