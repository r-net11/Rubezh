using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class DetailsParameterViewModel : BaseViewModel
	{
		public DetailsParameterViewModel(Parameter model, DeviceDetailsViewModel parent)
		{
			Model = model;
			Name = model.DriverParameter.Description;
			_parent = parent;
			IsNotReadOnly = !model.DriverParameter.IsReadOnly && (model.DriverParameter.CanWriteInActive || !parent.IsActive);

			if (IsNotReadOnly)
				switch (Model.DriverParameter.ParameterType)
				{
					case ParameterType.Enum:
						IsEnum = true;
						ParameterEnum = new ObservableCollection<ParameterEnumItem>(Model.DriverParameter.ParameterEnumItems);
						SelectedEnumItem = ParameterEnum.FirstOrDefault(x => x.Value == model.IntValue);
						if (SelectedEnumItem == null)
							SelectedEnumItem = ParameterEnum.FirstOrDefault();
						break;
					case ParameterType.String:
						IsString = true;
						StringValue = Model.StringValue;
						break;
					case ParameterType.Int:
						IsInt = true;
						IntValue = Model.IntValue ?? Model.DriverParameter.IntDefaultValue;
						if (Model.DriverParameter.IsReadOnly)
							StringValue = IntValue.ToString();
						break;
					case ParameterType.Double:
						IsDouble = true;
						var doubleValue = Model.DoubleValue ?? Model.DriverParameter.DoubleDefaultValue;
						StringValue = doubleValue.ToString();
						break;
					case ParameterType.Bool:
						IsBool = true;
						BoolValue = Model.BoolValue ?? Model.DriverParameter.BoolDefaultValue;
						break;
					case ParameterType.DateTime:
						IsDateTime = true;
						var dateTime = Model.DateTimeValue ?? DateTime.Now;
						DateTimeValue = new DateTimePairViewModel(dateTime);
						TimeSpan = dateTime.TimeOfDay;
						break;
					default:
						break;
				}
			else
				ReadOnlyValue = model.GetStringValue();
		}

		DeviceDetailsViewModel _parent;

		public string ReadOnlyValue { get; private set; }

		public bool IsNotReadOnly { get; private set; }

		public ObservableCollection<ParameterEnumItem> ParameterEnum { get; private set; }

		ParameterEnumItem _SelectedEnumItem;
		public ParameterEnumItem SelectedEnumItem
		{
			get { return _SelectedEnumItem; }
			set
			{
				_SelectedEnumItem = value;
				OnPropertyChanged(() => SelectedEnumItem);
			}
		}
		public bool IsEnum { get; private set; }

		public bool Save()
		{
			if(IsNotReadOnly)
				switch (Model.DriverParameter.ParameterType)
				{
					case ParameterType.Enum:
						Model.IntValue = SelectedEnumItem.Value;
						break;
					case ParameterType.String:
						Model.StringValue = StringValue;
						break;
					case ParameterType.Int:
						Model.IntValue = IntValue;
						break;
					case ParameterType.Double:
						double doubleValue;
						var stringValue = StringValue.Replace(',', '.');
						var parseResult = Double.TryParse(stringValue, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out doubleValue);
						if (!parseResult)
						{
							MessageBoxService.Show("Не могу определить значение вещественного параметра " + Name);
							return false;
						}
						Model.DoubleValue = doubleValue;
						break;
					case ParameterType.Bool:
						Model.BoolValue = BoolValue;
						break;
					case ParameterType.DateTime:
						Model.DateTimeValue = DateTimeValue.DateTime;
						break;
					default:
						break;
				}
			return true;
		}

		public string Name { get; private set; }
		public Parameter Model { get; private set; }

		int _IntValue;
		public int IntValue
		{
			get { return _IntValue; }
			set
			{
				_IntValue = value;
				OnPropertyChanged(() => IntValue);
			}
		}
		public bool IsInt { get; private set; }

		double _DoubleValue;
		public double DoubleValue
		{
			get { return _DoubleValue; }
			set
			{
				_DoubleValue = value;
				OnPropertyChanged(() => DoubleValue);
			}
		}
		public bool IsDouble { get; private set; }

		string _StringValue;
		public string StringValue
		{
			get { return _StringValue; }
			set
			{
				_StringValue = value;
				OnPropertyChanged(() => StringValue);
			}
		}
		public bool IsString { get; private set; }

		bool _BoolValue;
		public bool BoolValue
		{
			get { return _BoolValue; }
			set
			{
				_BoolValue = value;
				OnPropertyChanged(() => BoolValue);
			}
		}
		public bool IsBool { get; private set; }

		DateTimePairViewModel _DateTimeValue;
		public DateTimePairViewModel DateTimeValue
		{
			get { return _DateTimeValue; }
			set
			{
				_DateTimeValue = value;
				OnPropertyChanged(() => DateTimeValue);
			}
		}

		TimeSpan _TimeSpan;
		public TimeSpan TimeSpan
		{
			get { return _TimeSpan; }
			set
			{
				_TimeSpan = value;
				OnPropertyChanged(() => TimeSpan);
			}
		}

		public bool IsDateTime { get; private set; }

		public bool IsShowTextBlock { get { return IsDouble || IsString; } }
	}
}
