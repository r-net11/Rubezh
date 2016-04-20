using System;
using System.Linq;
using OpcClientSdk.Da;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public abstract class OpcTechnosoftwareTagValueBaseViewModel : BaseViewModel
	{
		#region Constructors

		public OpcTechnosoftwareTagValueBaseViewModel(TsCDaBrowseElement tag)
		{
			if (!tag.IsItem)
			{
				throw new ArgumentException("Элемент не является тегом", "tag");
			}
			Tag = tag;
		}

		#endregion

		#region Fields And Properties

		public TsCDaBrowseElement Tag { get; protected set; }

		bool _isEnabled;
		public bool IsEnabled 
		{
			get { return _isEnabled; }
			set 
			{ 
				_isEnabled = value;
				OnPropertyChanged(() => IsEnabled);
			} 
		}
		public TsCDaQuality ValueQuality 
		{
			get { return (TsCDaQuality)Tag.Properties.First(x => x.ID == TsDaProperty.QUALITY).Value; }
			set 
			{
				Tag.Properties.First(x => x.ID == TsDaProperty.QUALITY).Value = value;
				OnPropertyChanged(() => ValueQuality);
			}
		}
		public abstract ValueType Value { get; set; }
		public TsCDaItemValue Item 
		{
			get 
			{
				return new TsCDaItemValue
				{
					ItemName = Tag.ItemName,
					ItemPath = Tag.ItemPath,
					Quality = ValueQuality,
					QualitySpecified = true,
					Value = this.Value
				};
			} 
		}
		public TsDaAccessRights Accessibility
		{
			get 
			{
				var rights = Tag.Properties.First(x => x.ID == TsDaProperty.ACCESSRIGHTS).Value;
				if (Enum.IsDefined(typeof(TsDaAccessRights), rights))
				{
					return (TsDaAccessRights)rights;
				}
				else
				{
					throw new InvalidCastException(string.Format(
						"Ошибка при преобразование значения {0} в тип TsDaAccessRights", rights));
				}
			}
		}

		#endregion

		#region Methods

		public static OpcTechnosoftwareTagValueBaseViewModel Create(TsCDaBrowseElement tag)
		{
			var property = tag.Properties.First(x => x.ID == TsDaProperty.DATATYPE);

			if ((Type)property.Value == typeof(Boolean))
			{
				return new OpcTechnosoftwareTagValueBoolViewModel(tag);
			}
			else
			{
				throw new NotSupportedException(
					string.Format("Неудалось создать объект. Таг с типом данных {0} не поддерживается",
					property.Value.ToString()));
			}
			throw new NotImplementedException();
		}

		#endregion
	}
}