using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace LibraryModule.ViewModels
{
	public class PresenterKeyViewModel : SaveCancelDialogViewModel
	{
		public PresenterKeyViewModel(DriverProperty property)
		{
			Title = "Ключ представления";
			Property = property;
		}

		public DriverProperty Property { get; private set; }
		public bool IsString
		{
			get { return !IsBool && !IsEnum; }
		}
		public bool IsBool
		{
			get { return Property.DriverPropertyType == DriverPropertyTypeEnum.BoolType; }
		}
		public bool IsEnum
		{
			get { return Property.DriverPropertyType == DriverPropertyTypeEnum.EnumType; }
		}

		private DriverPropertyParameter _selectedParameter;
		public DriverPropertyParameter SelectedParameter
		{
			get { return _selectedParameter; }
			set
			{
				_selectedParameter = value;
				OnPropertyChanged(() => SelectedParameter);
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
		private string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged(() => Text);
			}
		}

		public string Value
		{
			get
			{
				switch (Property.DriverPropertyType)
				{
					case DriverPropertyTypeEnum.BoolType:
						return IsChecked.ToString();
					case DriverPropertyTypeEnum.EnumType:
						return SelectedParameter == null ? null : SelectedParameter.Value;
					default:
						return Text;
				}
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Value);
		}
	}
}