using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class DetailsParameterViewModel : BaseViewModel
	{
		public DetailsParameterViewModel(Parameter model)
		{
			Model = model;
			Name = model.DriverParameter.Name;
			IsNotReadOnly = !model.DriverParameter.IsReadOnly;
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
					IntValue = Model.IntValue != null ? Model.IntValue.Value : -1;
					break;
				case ParameterType.Double:
					IsDouble = true;
					DoubleValue = Model.DoubleValue != null ? Model.DoubleValue.Value : -1.0;
					break;
				case ParameterType.Bool:
					IsBool = true;
					BoolValue = Model.BoolValue;
					break;
				case ParameterType.DateTime:
					IsDateTime = true;
					var dateTime = Model.DateTimeValue != null ? Model.DateTimeValue.Value : DateTime.Now;
					DateTimeValue = new DateTimePairViewModel(dateTime);
					break;
				default:
					break;
			}
		}

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

		public void Save()
		{
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
					Model.DoubleValue = DoubleValue;
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
		public bool IsDateTime { get; private set; }

		public bool IsShowTextBlock { get { return IsDouble || IsString; } }
	}
}
