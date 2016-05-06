using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using StrazhAPI.GK;
using StrazhAPI.SKD;
using StrazhAPI.Models;
using StrazhAPI.Automation;
using FiresecClient;
using System.Collections.ObjectModel;
using Infrastructure.Common;

namespace AutomationModule.ViewModels
{
	public class ExplicitValueViewModel : BaseViewModel
	{
		public SKDDevice SKDDevice { get; private set; }
		public SKDZone SKDZone { get; private set; }
		public Camera Camera { get; private set; }
		public SKDDoor SKDDoor { get; private set; }
		public ExplicitValue ExplicitValue { get; private set; }

		public ExplicitValueViewModel()
		{
			ExplicitValue = new ExplicitValue();
			StateTypeValues = ProcedureHelper.GetEnumObs<XStateClass>();
		}

		public ExplicitValueViewModel(ExplicitValue explicitValue)
		{
			ExplicitValue = explicitValue;
			StateTypeValues = ProcedureHelper.GetEnumObs<XStateClass>();
			Initialize(ExplicitValue.UidValue);
		}

		public void Initialize(Guid uidValue)
		{
			SKDDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == uidValue);
			SKDZone = SKDManager.Zones.FirstOrDefault(x => x.UID == uidValue);
			Camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == uidValue);
			SKDDoor = SKDManager.Doors.FirstOrDefault(x => x.UID == uidValue);
			base.OnPropertyChanged(() => PresentationName);
		}

		public string PresentationName
		{
			get
			{
				if (SKDDevice != null)
					return SKDDevice.Name;
				if (SKDZone != null)
					return SKDZone.PresentationName;
				if (Camera != null)
					return Camera.PresentationName;
				if (SKDDoor != null)
					return SKDDoor.PresentationName;
				return "";
			}
		}

		public bool BoolValue
		{
			get { return ExplicitValue.BoolValue; }
			set
			{
				ExplicitValue.BoolValue = value;
				OnPropertyChanged(() => BoolValue);
			}
		}

		public DateTime DateTimeValue
		{
			get { return ExplicitValue.DateTimeValue; }
			set
			{
				ExplicitValue.DateTimeValue = value;
				OnPropertyChanged(() => DateTimeValue);
			}
		}

		public int IntValue
		{
			get { return ExplicitValue.IntValue; }
			set
			{
				ExplicitValue.IntValue = value;
				OnPropertyChanged(() => IntValue);
			}
		}

		public string StringValue
		{
			get { return ExplicitValue.StringValue; }
			set
			{
				ExplicitValue.StringValue = value;
				OnPropertyChanged(() => StringValue);
			}
		}

		public Guid UidValue
		{
			get { return ExplicitValue.UidValue; }
			set
			{
				ExplicitValue.UidValue = value;
				Initialize(value);
				OnPropertyChanged(() => UidValue);
				OnPropertyChanged(() => IsEmpty);
			}
		}

		public ObservableCollection<XStateClass> StateTypeValues { get; private set; }
		public XStateClass StateTypeValue
		{
			get { return ExplicitValue.StateTypeValue; }
			set
			{
				ExplicitValue.StateTypeValue = value;
				OnPropertyChanged(() => StateTypeValue);
			}
		}
		public bool IsEmpty
		{
			get
			{
				return ((SKDDevice == null) && (SKDZone == null) && (Camera == null) && (SKDDoor == null));
			}
			set
			{
				if (value)
					SKDDevice = null; SKDZone = null; Camera = null; SKDDoor = null;
			}
		}
	}
}
