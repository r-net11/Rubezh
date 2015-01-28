using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ConfigurationExportDetailsViewModel: BaseViewModel
	{
		public ConfigurationExportDetailsViewModel(ConfigurationExportFilter filter)
		{
			IsWithZones = filter.IsExportZones;
			IsWithDevices = filter.IsExportDevices;
			IsWithDoors = filter.IsExportDoors;
			Path = filter.Path;
		}

		bool _IsWithZones;
		public bool IsWithZones
		{
			get { return _IsWithZones; }
			set
			{
				_IsWithZones = value;
				OnPropertyChanged(() => IsWithZones);
			}
		}

		bool _IsWithDevices;
		public bool IsWithDevices
		{
			get { return _IsWithDevices; }
			set
			{
				_IsWithDevices = value;
				OnPropertyChanged(() => IsWithDevices);
			}
		}

		bool _IsWithDoors;
		public bool IsWithDoors
		{
			get { return _IsWithDoors; }
			set
			{
				_IsWithDoors = value;
				OnPropertyChanged(() => IsWithDoors);
			}
		}

		string _Path;
		public string Path
		{
			get { return _Path; }
			set
			{
				_Path = value;
				OnPropertyChanged(() => Path);
			}
		}

		public ConfigurationExportFilter Filter
		{
			get
			{
				return new ConfigurationExportFilter
				{
					IsExportZones = IsWithZones,
					IsExportDoors = IsWithDoors,
					IsExportDevices = IsWithDevices,
					Path = Path
				};
			}
		}
	}
}
