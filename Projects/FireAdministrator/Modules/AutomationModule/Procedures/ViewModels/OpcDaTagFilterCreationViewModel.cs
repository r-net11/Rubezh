using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Automation;
using RubezhClient;
using System;
using System.Collections.Generic;

namespace AutomationModule.ViewModels
{
	public class OpcDaTagFilterCreationViewModel : SaveCancelDialogViewModel
	{
		#region Constructors
		public OpcDaTagFilterCreationViewModel(OpcDaTagFileterSectionViewModel viewModel)
		{
			Title = "Создание фильтра";
			_opcDaTagFileterSectionViewModel = viewModel;
		}
		#endregion

		#region Fields And Properties

		OpcDaTagFileterSectionViewModel _opcDaTagFileterSectionViewModel;
		public List<OpcDaServer> OpcDaServers
		{
			get { return ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTsServers; }
		}

		OpcDaServer _selectedOpcDaServer;
		public OpcDaServer SelectedOpcDaServer
		{
			get { return _selectedOpcDaServer; }
			set
			{
				_selectedOpcDaServer = value;
				OnPropertyChanged(() => SelectedOpcDaServer);
				OnPropertyChanged(() => OpcDaTags);
			}
		}

		public List<OpcDaTag> OpcDaTags
		{
			get
			{
				var tags = new List<OpcDaTag>();

				if (_selectedOpcDaServer != null)
				{
					foreach (var tag in _selectedOpcDaServer.Tags)
					{
						if (null != OpcDaTagFilter.GetExplicitType(tag.TypeNameOfValue))
							tags.Add(tag);
					}
				}
				return tags;
			}
		}

		OpcDaTag _selectedOpcDaTag;
		public OpcDaTag SelectedOpcDaTag
		{
			get { return _selectedOpcDaTag; }
			set
			{
				_selectedOpcDaTag = value;
				if (value != null)
				{
					var type = OpcDaTagFilter.GetExplicitType(_selectedOpcDaTag.TypeNameOfValue);

					if (null != type)
					{
						switch (type.Value)
						{
							case ExplicitType.Integer:
							case ExplicitType.Float:
								{
									HysterasisEnabled = true; break;
								}
							default:
								{
									HysterasisEnabled = false; break;
								}
						}
					}
					else
					{
						HysterasisEnabled = false;
					}
				}
				OnPropertyChanged(() => SelectedOpcDaTag);
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

		uint _hysteresis;
		public uint Hysteresis
		{
			get { return _hysteresis; }
			set
			{
				_hysteresis = value;
				OnPropertyChanged(() => Hysteresis);
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		bool _hysterasisEnabled;
		public bool HysterasisEnabled
		{
			get { return _hysterasisEnabled; }
			private set
			{
				_hysterasisEnabled = value;
				OnPropertyChanged(() => HysterasisEnabled);
			}
		}

		public OpcTagFilterViewModel OpcDaTagFilterResult { get; private set; }

		#endregion

		#region Methods
		protected override bool CanSave()
		{
			//return base.CanSave();
			return (_selectedOpcDaTag != null) &&
				(!String.IsNullOrEmpty(Name)) && (!String.IsNullOrWhiteSpace(Name));
		}

		protected override bool Save()
		{
			var type = OpcDaTagFilter.GetExplicitType(SelectedOpcDaTag.TypeNameOfValue);

			if (type != null)
			{
				OpcDaTagFilterResult =
					new OpcTagFilterViewModel(new OpcDaTagFilter(Guid.NewGuid(), Name,
						Description == null ? string.Empty : Description,
						SelectedOpcDaTag.Uid, Hysteresis, type.Value));
				return true;
			}
			else
			{
				return false;
			}
			//return base.Save();
		}
		#endregion
	}
}