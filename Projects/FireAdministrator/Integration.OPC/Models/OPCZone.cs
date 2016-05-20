using Infrastructure.Common.Windows.ViewModels;
using StrazhAPI.Enums;

namespace Integration.OPC.Models
{
	public class OPCZone : BaseViewModel
	{

		#region Fields
		private readonly int?  _autoSet;
		private readonly int?  _delay;
		private readonly GuardZoneType? _guardZoneType;
		private readonly bool? _isSkippedTypeEnabled;
		#endregion

		#region Properties
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

		public OPCZone(StrazhAPI.Integration.OPC.OPCZone zone, bool isExist)
		{
			if (zone == null) return;

			_autoSet = zone.AutoSet;
			_delay = zone.Delay;
			_guardZoneType = zone.GuardZoneType;
			_isSkippedTypeEnabled = zone.IsSkippedTypeEnabled;

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

			_autoSet = zone.AutoSet;
			_delay = zone.Delay;
			_guardZoneType = zone.GuardZoneType;
			_isSkippedTypeEnabled = zone.IsSkippedTypeEnabled;

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
				AutoSet = _autoSet,
				Delay = _delay,
				Description = Description,
				GuardZoneType = _guardZoneType,
				IsSkippedTypeEnabled = _isSkippedTypeEnabled,
				Name = Name,
				No = No,
				Type = Type
			};
		}
	}
}
