using Infrastructure.Common.Windows.ViewModels;
using StrazhAPI.Enums;

namespace Integration.OPC.Models
{
	public class OPCZone : BaseViewModel
	{
		private bool? _isSkippedTypeEnabled;
		private int? _delay;
		private int? _autoset;
		private GuardZoneType? _guardZoneType;
		private int _no;
		private string _name;
		private OPCZoneType _type;
		private string _description;
		private bool _isChecked;
		private bool _isEnabled;

		public bool? IsSkippedTypeEnabled
		{
			get { return _isSkippedTypeEnabled; }
			set
			{
				_isSkippedTypeEnabled = value;
				OnPropertyChanged(() => IsSkippedTypeEnabled);
			}
		}

		public int? Delay
		{
			get { return _delay; }
			set
			{
				_delay = value;
				OnPropertyChanged(() => Delay);
			}
		}

		public int? AutoSet
		{
			get { return _autoset; }
			set
			{
				_autoset = value;
				OnPropertyChanged(() => AutoSet);
			}
		}

		public GuardZoneType? GuardZoneType
		{
			get { return _guardZoneType; }
			set
			{
				_guardZoneType = value;
				OnPropertyChanged(() => GuardZoneType);
			}
		}

		public int No
		{
			get { return _no; }
			set
			{
				_no = value;
				OnPropertyChanged(() => No);
			}
		}

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		public OPCZoneType Type
		{
			get { return _type; }
			set
			{
				_type = value;
				OnPropertyChanged(() => Type);
			}
		}

		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}

		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				_isEnabled = value;
				OnPropertyChanged(() => IsEnabled);
			}
		}

		/// <summary>
		/// Свойство, которое должно использоваться, при добавлении экземпляров данного класса.
		/// </summary>
		public bool CanAdd
		{
			get { return IsChecked && IsEnabled; }
		}

		public OPCZone(StrazhAPI.Integration.OPC.OPCZone zone, bool isExist)
		{
			if (zone == null) return;

			AutoSet = zone.AutoSet;
			Delay = zone.Delay;
			GuardZoneType = zone.GuardZoneType;
			IsSkippedTypeEnabled = zone.IsSkippedTypeEnabled;
			No = zone.No;
			Name = zone.Name;
			if (zone.Type != null)
				Type = (OPCZoneType) zone.Type;
			Description = zone.Description;
			IsChecked = isExist;
			IsEnabled = !isExist;
		}

		public OPCZone(StrazhAPI.Integration.OPC.OPCZone zone)
		{
			if (zone == null) return;

			AutoSet = zone.AutoSet;
			Delay = zone.Delay;
			GuardZoneType = zone.GuardZoneType;
			IsSkippedTypeEnabled = zone.IsSkippedTypeEnabled;
			No = zone.No;
			Name = zone.Name;
			if (zone.Type != null)
				Type = (OPCZoneType)zone.Type;
			Description = zone.Description;
		}

		public StrazhAPI.Integration.OPC.OPCZone ToDTO()
		{
			return new StrazhAPI.Integration.OPC.OPCZone
			{
				AutoSet = AutoSet,
				Delay = Delay,
				Description = Description,
				GuardZoneType = GuardZoneType,
				IsSkippedTypeEnabled = IsSkippedTypeEnabled,
				Name = Name,
				No = No,
				Type = Type
			};
		}
	}
}
