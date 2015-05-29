using System;
using FiresecAPI.GK;
using GKImitator.Processor;
using GKProcessor;
using Infrastructure.Common.Windows.ViewModels;

namespace GKImitator.ViewModels
{
	public class StateBitViewModel : BaseViewModel
	{
		public GKStateBit StateBit { get; private set; }
		BinaryObjectViewModel BinaryObjectViewModel;

		public StateBitViewModel(BinaryObjectViewModel binaryObjectViewModel, GKStateBit stateBit, bool isActive = false)
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
					OnPropertyChanged(() => IsActive);

					var state = 0;
					foreach (var stateBitViewModel in BinaryObjectViewModel.StateBits)
					{
						if (stateBitViewModel.IsActive)
						{
							state += (1 << (int)stateBitViewModel.StateBit);
						}
					}

					var binaryObject = BinaryObjectViewModel.BinaryObject;

					switch ((BinaryObjectViewModel.BinaryObject.GKBase as GKDevice).Driver.DriverType)
					{
						case GKDriverType.RSR2_HandDetector:
							switch (StateBit)
							{
								case GKStateBit.Fire2:
									if (_isActive)
									{
										AddJournalItem(binaryObject, 3, 0, 0, state);
										AddJournalItem(binaryObject, 3, 1, 0, state);
									}
									else
									{
										AddJournalItem(binaryObject, 14, 0, 0, state);
									}
									break;
							}
							break;

						case GKDriverType.GKIndicator:
							switch (StateBit)
							{
								case GKStateBit.On:
									if (_isActive)
									{
										AddJournalItem(binaryObject, 9, 2, 0, state);
									}
									else
									{
										AddJournalItem(binaryObject, 9, 3, 0, state);
									}
									break;
							}
							break;
					}

					MainViewModel.Current.RebuildIndicators();
				}
			}
		}

		void AddJournalItem(BaseDescriptor baseDescriptor, byte code, byte eventDescription, byte eventYesNo, int objectState)
		{
			var imitatorJournalItem = new ImitatorJournalItem();
			imitatorJournalItem.DateTime = DateTime.Now;
			imitatorJournalItem.GkNo = JournalHelper.ImitatorJournalItemCollection.ImitatorJournalItems.Count + 1;
			imitatorJournalItem.GkObjectNo = baseDescriptor.GetDescriptorNo();

			imitatorJournalItem.Source = 2;
			imitatorJournalItem.Code = code;
			imitatorJournalItem.EventDescription = eventDescription;
			imitatorJournalItem.EventYesNo = eventYesNo;

			imitatorJournalItem.ObjectNo = 0;
			imitatorJournalItem.ObjectDeviceType = 0;
			imitatorJournalItem.ObjectDeviceAddress = 0;
			imitatorJournalItem.ObjectFactoryNo = 0;
			imitatorJournalItem.ObjectState = objectState;

			if (baseDescriptor.GKBase is GKDevice)
			{
				imitatorJournalItem.ObjectDeviceType = (short)(baseDescriptor.GKBase as GKDevice).Driver.DriverTypeNo;
				imitatorJournalItem.ObjectDeviceAddress = (short)(((baseDescriptor.GKBase as GKDevice).ShleifNo - 1) * 256 + (baseDescriptor.GKBase as GKDevice).IntAddress);
			}

			JournalHelper.ImitatorJournalItemCollection.ImitatorJournalItems.Add(imitatorJournalItem);
			JournalHelper.Save();
		}
	}
}