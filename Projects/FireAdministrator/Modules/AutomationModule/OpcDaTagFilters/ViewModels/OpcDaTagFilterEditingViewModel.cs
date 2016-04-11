using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Automation;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class OpcDaTagFilterEditingViewModel : SaveCancelDialogViewModel, IDataErrorInfo
	{
		#region Constructors

		public OpcDaTagFilterEditingViewModel(OpcTagFilterViewModel filter)
		{
			Title = "Редактирование фильтра OPC DA тега";
			SelectedOpcDaTagFilter = filter;

			_Filters = ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTagFilters
			.Where(f => f.UID != filter.OpcDaTagFilter.UID);

			if ((filter.OpcDaServer == null) || (filter.OpcDaTag == null))
			{
				Name = SelectedOpcDaTagFilter.OpcDaTagFilter.Name;
				Description = SelectedOpcDaTagFilter.OpcDaTagFilter.Description;
				Hysteresis = SelectedOpcDaTagFilter.OpcDaTagFilter.Hysteresis.ToString();
			}
			else
			{
				SelectedOpcDaServer = OpcDaServers
					.FirstOrDefault(srv => srv.Tags
						.Any(tag => tag.Uid == SelectedOpcDaTagFilter.OpcDaTagFilter.TagUID));

				if (SelectedOpcDaServer != null)
				{
					SelectedOpcDaTag = OpcDaServers.SelectMany(srv => srv.Tags)
						.FirstOrDefault(tag => tag.Uid == SelectedOpcDaTagFilter.OpcDaTagFilter.TagUID);
					Name = SelectedOpcDaTagFilter.OpcDaTagFilter.Name;
					Description = SelectedOpcDaTagFilter.OpcDaTagFilter.Description;
					Hysteresis = SelectedOpcDaTagFilter.OpcDaTagFilter.Hysteresis.ToString();
				}
			}
		}

		#endregion

		#region Fields And Properties

		IEnumerable<OpcDaTagFilter> _Filters;

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
							case ExplicitType.Boolean:
								{
									Hysteresis = String.Empty;
									HysterasisEnabled = false; break;
								}
							case ExplicitType.Integer:
							case ExplicitType.Float:
								{
									HysterasisEnabled = true; break;
								}
						}
					}
					else
					{
						HysterasisEnabled = false;
					}
				}
				// Запускаем валидацию значения
				OnPropertyChanged(() => Hysteresis);
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

		string _hysteresis;
		public string Hysteresis
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

		string _errorMessageByName;
		public string ErrorMessageByName
		{
			get { return _errorMessageByName; }
			set
			{
				_errorMessageByName = value;
				OnPropertyChanged(() => ErrorMessageByName);
			}
		}

		string _errorMessageByHystersis;
		public string ErrorMessageByHystersis
		{
			get { return _errorMessageByHystersis; }
			private set
			{
				_errorMessageByHystersis = value;
				OnPropertyChanged(() => ErrorMessageByHystersis);
			}
		}

		public string Error
		{
			get { return null; }
		}

		public string this[string columnName]
		{
			get
			{
				string message;

				switch (columnName)
				{
					case "Name":
						{
							if (String.IsNullOrEmpty(Name.Trim()))
							{
								message = "Название фильтра не может быть пустым";
								ErrorMessageByName = message;
								return message;
							}
							if (Name.Length > 30)
							{
								message = "Название фильтра не может быть более 30 символов";
								ErrorMessageByName = message;
								return message;
							}
							if (_Filters.Any(filter => filter.Name == Name))
							{
								message = "Фильтр с данным название уже существует";
								ErrorMessageByName = message;
								return message;
							}
							else
							{
								ErrorMessageByName = null;
								return null;
							}
						}
					case "Hysteresis":
						{
							if (SelectedOpcDaTag == null)
							{
								message = "Невозможно определить тип данных для значения гистререзиса, невыбран тег";
								ErrorMessageByHystersis = message;
								return message;
							}

							var type = OpcDaTagFilter.GetExplicitType(_selectedOpcDaTag.TypeNameOfValue);

							if (!type.HasValue)
							{
								message = "Невозножно установить значение. Тег с данным типом данных не поддерживается";
								ErrorMessageByHystersis = message;
								return message;
							}

							switch (type.Value)
							{
								case ExplicitType.Boolean:
									{
										ErrorMessageByHystersis = null;
										return null;
									}
								case ExplicitType.Integer:
									{
										Int32 x;
										if (Int32.TryParse(Hysteresis, out x))
										{
											if (x < 0)
											{
												message = "Значение гистерезиса неверно";
												ErrorMessageByHystersis = message;
												return message;
											}
											else
											{
												ErrorMessageByHystersis = null;
												return null;
											}
										}
										else
										{
											message = "Значение гистерезиса неверно";
											ErrorMessageByHystersis = message;
											return message;
										}
									}
								case ExplicitType.Float:
									{
										double x;
										if (Double.TryParse(Hysteresis, out x))
										{
											if (x < 0)
											{
												message = "Значение гистерезиса неверно";
												ErrorMessageByHystersis = message;
												return message;
											}
											else
											{
												ErrorMessageByHystersis = null;
												return null;
											}
										}
										else
										{
											message = "Значение гистерезиса неверно";
											ErrorMessageByHystersis = message;
											return message;
										}
									}
								default:
									{
										message = "Невозможно установить значение. Фильтр для тега с данным типом данных не поддерживается";
										ErrorMessageByHystersis = message;
										return message;
									}
							}
						}
					default: { return null; }
				}
			}
		}

		#endregion

		#region Methods

		protected override bool CanSave()
		{
			//return base.CanSave();
			return (SelectedOpcDaServer != null) && (SelectedOpcDaTag != null)
				&& (ErrorMessageByName == null) && (ErrorMessageByHystersis == null);
		}

		protected override bool Save()
		{
			SelectedOpcDaTagFilter.OpcDaTagFilter.TagUID = SelectedOpcDaTag.Uid;
			SelectedOpcDaTagFilter.OpcDaServer = SelectedOpcDaServer;
			SelectedOpcDaTagFilter.OpcDaTag = SelectedOpcDaTag;
			SelectedOpcDaTagFilter.Name = Name;
			SelectedOpcDaTagFilter.Hysteresis = String.IsNullOrEmpty(Hysteresis) ? 0 : Double.Parse(Hysteresis);
			SelectedOpcDaTagFilter.Description = Description;
			SelectedOpcDaTagFilter.OpcDaTagFilter.ValueType =
				OpcDaTagFilter.GetExplicitType(SelectedOpcDaTag.TypeNameOfValue).Value;
			return true;
			//return base.Save();
		}

		#endregion
	}
}