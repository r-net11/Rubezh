using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Automation;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutomationModule.ViewModels
{
	public class OpcDaTagFilterEditingViewModel : SaveCancelDialogViewModel
	{
		#region Constructors

		public OpcDaTagFilterEditingViewModel(OpcTagFilterViewModel filter)
		{
			Title = "Редактирование фильтра OPC DA тега";
			SelectedOpcDaTagFilter = filter;

			foreach(var server in OpcDaServers)
			{
				SelectedOpcDaTag = server.Tags.FirstOrDefault(tag => tag.Uid == SelectedOpcDaTagFilter.OpcDaTagFilter.TagUID);
				if (SelectedOpcDaTag != null)
				{
					SelectedOpcDaServer = server;
					Name = SelectedOpcDaTagFilter.OpcDaTagFilter.Name;
					Description = SelectedOpcDaTagFilter.OpcDaTagFilter.Description;
					Hysteresis = SelectedOpcDaTagFilter.OpcDaTagFilter.Hysteresis;
					break;
				}
			}
		}

		#endregion

		#region Fields And Properties

		public OpcTagFilterViewModel SelectedOpcDaTagFilter { get; private set; }

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

		double _hysteresis;
		public double Hysteresis
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

		#endregion

		#region Methods
		protected override bool CanSave()
		{
			//return base.CanSave();
			return (SelectedOpcDaServer != null) && (SelectedOpcDaTag != null);
		}

		protected override bool Save()
		{
			//var type = OpcDaTagFilter.GetExplicitType(SelectedOpcDaTag.TypeNameOfValue);
			SelectedOpcDaTagFilter.OpcDaTagFilter.TagUID = SelectedOpcDaTag.Uid;
			SelectedOpcDaTagFilter.OpcDaServer = SelectedOpcDaServer;
			SelectedOpcDaTagFilter.OpcDaTag = SelectedOpcDaTag;
			SelectedOpcDaTagFilter.Name = Name;
			SelectedOpcDaTagFilter.Hysteresis = Hysteresis;
			SelectedOpcDaTagFilter.Description = Description;
			return true;
			//return base.Save();
		}

		#endregion
	}
}