using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;
using System.Linq;
using FiresecAPI.SKD;
using FiresecAPI.GK;
using FiresecAPI.Models;
using Infrastructure;
using System.Linq.Expressions;

namespace AutomationModule.ViewModels
{
	public class VariableViewModel : BaseViewModel
	{
		public Variable Variable { get; set; }
		public VariableItemViewModel VariableItem { get; private set; }

		public VariableViewModel(Variable variable)
		{
			Variable = variable;
			ObjectTypes = ProcedureHelper.GetEnumObs<ObjectType>();
			EnumTypes = ProcedureHelper.GetEnumObs<EnumType>();
			VariableItem = new VariableItemViewModel(variable.DefaultVariableItem);
			Update();
		}

		public void Update()
		{
			Name = Variable.Name;
			IsList = Variable.IsList;
			ExplicitType = Variable.ExplicitType;
			EnumType = Variable.EnumType;
			ObjectType = Variable.ObjectType;

			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => IsList);
			OnPropertyChanged(() => ExplicitType);
			OnPropertyChanged(() => EnumType);
			OnPropertyChanged(() => ObjectType);
		}

		bool _isList;
		public bool IsList
		{
			get { return _isList; }
			set
			{
				_isList = value;
				OnPropertyChanged(() => IsList);
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		ExplicitType _ExplicitType;
		public ExplicitType ExplicitType
		{
			get { return _ExplicitType; }
			set
			{
				_ExplicitType = value;
				OnPropertyChanged(() => ExplicitType);
			}
		}

		public ObservableCollection<ObjectType> ObjectTypes { get; private set; }
		ObjectType _objectType;
		public ObjectType ObjectType
		{
			get { return _objectType; }
			set
			{
				_objectType = value;
				OnPropertyChanged(() => ObjectType);
			}
		}

		public ObservableCollection<EnumType> EnumTypes { get; private set; }
		EnumType _enumType;
		public EnumType EnumType
		{
			get { return _enumType; }
			set
			{
				_enumType = value;
				OnPropertyChanged(() => EnumType);
			}
		}
	}

	public class VariableItemViewModel : BaseViewModel
	{
		public XDevice Device { get; private set; }
		public XZone Zone { get; private set; }
		public XGuardZone GuardZone { get; private set; }
		public SKDDevice SKDDevice { get; private set; }
		public SKDZone SKDZone { get; private set; }
		public Camera Camera { get; private set; }
		public SKDDoor SKDDoor { get; private set; }
		public XDirection Direction { get; private set; }
		public VariableItem VariableItem { get; private set; }
		public Action UpdateDescriptionHandler { get; set; }

		public VariableItemViewModel(VariableItem variableItem)
		{
			VariableItem = new VariableItem();
			Initialize(variableItem);
		}

		public void Initialize(VariableItem variableItem)
		{
			VariableItem = (VariableItem)variableItem.Clone();
			StateClassValues = ProcedureHelper.GetEnumObs<XStateClass>();
			DeviceTypes = new ObservableCollection<string>(XManager.Drivers.Select(x => x.Name));
			DeviceType = DeviceType ?? DeviceTypes.FirstOrDefault();
			var objectUid = variableItem.UidValue;
			Device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == objectUid);
			Zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == objectUid);
			GuardZone = XManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == objectUid);
			SKDDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == objectUid);
			SKDZone = SKDManager.Zones.FirstOrDefault(x => x.UID == objectUid);
			Camera = FiresecManager.SystemConfiguration.AllCameras.FirstOrDefault(x => x.UID == objectUid);
			SKDDoor = SKDManager.Doors.FirstOrDefault(x => x.UID == objectUid);
			Direction = XManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == objectUid);
		}

		public string PresentationName
		{
			get
			{
				if (Device != null)
					return Device.PresentationName;
				if (Zone != null)
					return Zone.PresentationName;
				if (GuardZone != null)
					return GuardZone.PresentationName;
				if (SKDDevice != null)
					return SKDDevice.Name;
				if (SKDZone != null)
					return SKDZone.Name;
				if (Camera != null)
					return Camera.Name;
				if (SKDDoor != null)
					return SKDDoor.Name;
				if (Direction != null)
					return Direction.PresentationName;
				return "";
			}
		}

		public bool BoolValue
		{
			get { return VariableItem.BoolValue; }
			set
			{
				VariableItem.BoolValue = value;
				OnPropertyChanged(() => BoolValue);
			}
		}

		public DateTime DateTimeValue
		{
			get { return VariableItem.DateTimeValue; }
			set
			{
				VariableItem.DateTimeValue = value;
				OnPropertyChanged(() => DateTimeValue);
			}
		}

		public int IntValue
		{
			get { return VariableItem.IntValue; }
			set
			{
				VariableItem.IntValue = value;
				OnPropertyChanged(() => IntValue);
			}
		}

		public string StringValue
		{
			get { return VariableItem.StringValue; }
			set
			{
				VariableItem.StringValue = value;
				OnPropertyChanged(() => StringValue);
			}
		}

		public ObservableCollection<XStateClass> StateClassValues { get; private set; }
		public XStateClass StateClassValue
		{
			get { return VariableItem.StateClassValue; }
			set
			{
				VariableItem.StateClassValue = value;
				OnPropertyChanged(() => StateClassValue);
			}
		}

		public ObservableCollection<string> DeviceTypes { get; private set; }
		public string DeviceType
		{
			get { return VariableItem.DeviceType; }
			set
			{
				VariableItem.DeviceType = value;
				OnPropertyChanged(() => DeviceType);
			}
		}

		public bool IsEmpty
		{
			get
			{
				return ((Device == null) && (Zone == null) && (GuardZone == null) && (SKDDevice == null) && (SKDZone == null) && (Camera == null) && (Direction == null));
			}
			set
			{
				if (value)
					Device = null; Zone = null; GuardZone = null; SKDDevice = null; SKDZone = null; Camera = null; Direction = null;
			}
		}

		public new void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
		{
			ServiceFactory.SaveService.AutomationChanged = true;
			base.OnPropertyChanged(propertyExpression);
			if (UpdateDescriptionHandler != null)
			    UpdateDescriptionHandler();
		}
	}
}