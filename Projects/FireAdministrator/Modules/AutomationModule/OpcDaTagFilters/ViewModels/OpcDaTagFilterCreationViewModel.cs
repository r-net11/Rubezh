using Infrastructure.Common.Services;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using RubezhAPI.Automation;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class OpcDaTagFilterCreationViewModel : SaveCancelDialogViewModel, IDataErrorInfo
	{
		#region Constructors
		public OpcDaTagFilterCreationViewModel()
		{
			Title = "Создание фильтра";
			Description = String.Empty;
			Name = String.Empty;
		}
		#endregion

		#region Fields And Properties

		public List<OpcDaServer> OpcDaServers
		{
			get { return ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTsServers; }
		}

		List<OpcDaTagFilter> _Filters = 
			ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTagFilters;

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
						var type = OpcDaTagFilter.GetExplicitType(tag.TypeNameOfValue);
						if (null != type)
							if ((type.Value == ExplicitType.Boolean) || (type.Value == ExplicitType.Float) ||
								(type.Value == ExplicitType.Integer) || (type.Value == ExplicitType.Double))
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
									HysterasisEnabled = true;
									if (String.IsNullOrEmpty(Hysteresis))
										Hysteresis = "0";
									else
									{
										// запускаем валидацию значения
										OnPropertyChanged(() => Hysteresis);
									}
									break;
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
				ErrorMessageByHystersis = this["Hysteresis"];
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
				OnPropertyChanged(() => Error);
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
				OnPropertyChanged(() => Error);
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
			bool canSave = false;
			canSave = _selectedOpcDaTag != null && _selectedOpcDaServer != null 
				&& ErrorMessageByName == null && ErrorMessageByHystersis == null;

			return canSave;
		}

		protected override bool Save()
		{
			double hysteresis;

			var type = OpcDaTagFilter.GetExplicitType(SelectedOpcDaTag.TypeNameOfValue);

			if (type != null)
			{
				switch (type.Value)
				{
					case ExplicitType.Integer:
						{
							hysteresis = Double.Parse(Hysteresis); break;
						}
					case ExplicitType.Float:
					case ExplicitType.Double:
						{
							hysteresis = Double.Parse(Hysteresis); break;
						}
					case ExplicitType.Boolean:
						{
							hysteresis = 0; break;
						}
					default:
						{
							return false;
						}
				}

				OpcDaTagFilterResult =
					new OpcTagFilterViewModel(new OpcDaTagFilter(Guid.NewGuid(), Name,
						Description == null ? string.Empty : Description,
						SelectedOpcDaTag.Uid, hysteresis, type.Value));
				return true;
			}
			else
			{
				return false;
			}
			//return base.Save();
		}
		#endregion

		string _errorMessageByName;
		public string ErrorMessageByName
		{
			get { return _errorMessageByName; }
			private set
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
								case ExplicitType.Double:
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
	}
}