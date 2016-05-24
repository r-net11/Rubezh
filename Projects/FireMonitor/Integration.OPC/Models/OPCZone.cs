using Infrastructure.Common.Windows.ViewModels;
using StrazhAPI.Enums;

namespace Integration.OPC.Models
{
	public class OPCZone : BaseViewModel
	{
		#region Properties

		private bool? _isSkippedTypeEnabled;

		public bool? IsSkippedTypeEnabled
		{
			get { return _isSkippedTypeEnabled; }
			set
			{
				_isSkippedTypeEnabled = value;
				OnPropertyChanged(() => IsSkippedTypeEnabled);
			}
		}

		private int? _delay;
		public int? Delay
		{
			get { return _delay; }
			set
			{
				_delay = value;
				OnPropertyChanged(() => Delay);
			}
		}

		private int? _autoset;
		public int? AutoSet
		{
			get { return _autoset; }
			set
			{
				_autoset = value;
				OnPropertyChanged(() => AutoSet);
			}
		}

		private GuardZoneType? _guardZoneType;

		public GuardZoneType? GuardZoneType
		{
			get { return _guardZoneType; }
			set
			{
				_guardZoneType = value;
				OnPropertyChanged(() => GuardZoneType);
			}
		}
		private int _no;

		public int No
		{
			get { return _no; }
			set
			{
				_no = value;
				OnPropertyChanged(() => No);
			}
		}

		private string _name;

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		private OPCZoneType _type;

		public OPCZoneType Type
		{
			get { return _type; }
			set
			{
				_type = value;
				OnPropertyChanged(() => Type);
			}
		}

		private string _description;

		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		private bool _isChecked;

		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}

		private bool _isEnabled;

		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				_isEnabled = value;
				OnPropertyChanged(() => IsEnabled);
			}
		}
		#endregion

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
