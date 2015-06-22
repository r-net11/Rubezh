using System;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class ObjectViewModel : BaseViewModel, IComparable
	{
		public string Name { get; set; }
		public string Address { get; set; }
		public string PresentationZone { get; set; }
		public bool IsAbsent { get; set; }
		public bool IsPresent { get; set; }
		public bool HasNonStructureDifferences
		{
			get
			{
				if (IsAbsent || IsPresent || !ConfigurationCompareViewModel.ConfigFromFile)
					return false;
				return !String.IsNullOrEmpty(DifferenceDiscription);
			}
		}

		public bool HasDifferences
		{
			get
			{
				return !String.IsNullOrEmpty(DifferenceDiscription);
			}
		}

		public string DifferenceDiscription { get; set; }
		public GKDevice Device;
		public string ImageSource { get; private set; }
		public ObjectType ObjectType { get; private set; }
		public object Clone()
		{
			return MemberwiseClone();
		}

		public ObjectViewModel(GKDevice device)
		{
			Device = device;
			Name = device.ShortName;
			Address = device.DottedPresentationAddress;
			ImageSource = "/Controls;component/GKIcons/" + device.DriverType + ".png";
			ObjectType = ObjectType.Device;
		}

		int IComparable.CompareTo(object a)
		{
			return Compare(this, a);
		}

		public int Compare(object a, object b)
		{
			var object1 = (ObjectViewModel)a;
			var object2 = (ObjectViewModel)b;
			if (object1.ObjectType > object2.ObjectType)
				return 1;
			if (object1.ObjectType < object2.ObjectType)
				return -1;

			if (object1.ObjectType == ObjectType.Device)
			{
				var orderNo1 =
					(object1.Device.KAUParent != null ? object1.Device.KAUParent.IntAddress * 256 * 256 * 256 : 0) +
					(object1.Device.ShleifNo * 256 * 256) +
					(!object1.Device.Driver.IsKau ? object1.Device.IntAddress * 256 : 0)
					+ object1.Device.Driver.DriverType;
				var orderNo2 =
					(object2.Device.KAUParent != null ? object2.Device.KAUParent.IntAddress * 256 * 256 * 256 : 0) +
					(object2.Device.ShleifNo * 256 * 256) +
					(!object2.Device.Driver.IsKau ? object2.Device.IntAddress * 256 : 0)
					+ object2.Device.Driver.DriverType;
				if (orderNo1 > orderNo2)
					return 1;
				if (orderNo1 < orderNo2)
					return -1;
				return 0;
			}
			return 0;
		}
	}

	public enum ObjectType
	{
		Device = 0,
	}
}