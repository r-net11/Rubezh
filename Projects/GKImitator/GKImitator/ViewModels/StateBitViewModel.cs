using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using GKImitator.Processor;
using Common.GK;

namespace GKImitator.ViewModels
{
	public class StateBitViewModel : BaseViewModel
	{
		public XStateBit StateBit { get; private set; }
		BinaryObjectViewModel BinaryObjectViewModel;

		public StateBitViewModel(BinaryObjectViewModel binaryObjectViewModel, XStateBit stateBit, bool isActive = false)
		{
			BinaryObjectViewModel = binaryObjectViewModel;
			StateBit = stateBit;
			_isActive = isActive;
		}

		bool _isActive;
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				if (_isActive != value)
				{
					_isActive = value;
					OnPropertyChanged("IsActive");

					var state = 0;
					foreach (var stateBitViewModel in BinaryObjectViewModel.StateBits)
					{
						if (stateBitViewModel.IsActive)
						{
							state += (1 << (int)stateBitViewModel.StateBit);
						}
					}

					var binaryObject = BinaryObjectViewModel.BinaryObject;
					switch (StateBit)
					{
						case XStateBit.Fire1:
							AddJournalItem(binaryObject, 2, 2, 1, state);
							break;

						case XStateBit.Fire2:
							AddJournalItem(binaryObject, 2, 2, 1, state);
							break;
					}

				}
			}
		}

		void AddJournalItem(BinaryObjectBase binaryObject, byte code, byte eventDescription, byte eventYesNo, int objectState)
		{
			var imitatorJournalItem = new ImitatorJournalItem();
			imitatorJournalItem.DateTime = DateTime.Now;
			imitatorJournalItem.GkNo = JournalHelper.ImitatorJournalItemCollection.ImitatorJournalItems.Count + 1;
			imitatorJournalItem.GkObjectNo = binaryObject.GkDescriptorNo;

			imitatorJournalItem.Source = 2;
			imitatorJournalItem.Code = code;
			imitatorJournalItem.EventDescription = eventDescription;
			imitatorJournalItem.EventYesNo = eventYesNo;

			imitatorJournalItem.ObjectNo = 0;
			imitatorJournalItem.ObjectDeviceType = 0;
			imitatorJournalItem.ObjectDeviceAddress = 0;
			imitatorJournalItem.ObjectFactoryNo = 0;
			imitatorJournalItem.ObjectState = objectState;

			if (binaryObject.Device != null)
			{
				imitatorJournalItem.ObjectDeviceType = (short)binaryObject.Device.Driver.DriverTypeNo;
				imitatorJournalItem.ObjectDeviceAddress = (short)((binaryObject.Device.ShleifNo - 1) * 256 + binaryObject.Device.IntAddress);
			}

			JournalHelper.ImitatorJournalItemCollection.ImitatorJournalItems.Add(imitatorJournalItem);
			JournalHelper.Save();
		}
	}
}