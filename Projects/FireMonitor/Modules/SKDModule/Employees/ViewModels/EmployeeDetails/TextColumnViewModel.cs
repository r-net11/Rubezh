using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class TextColumnViewModel: BaseViewModel
	{
		public AdditionalColumn AdditionalColumn { get; private set; }
		public AdditionalColumnType AdditionalColumnType { get; private set; }
		public string Name
		{
			get { return AdditionalColumnType.Name; }
		}
		public string Text
		{
			get { return AdditionalColumn.TextData; }
			set
			{
				AdditionalColumn.TextData = value;
				OnPropertyChanged(() => Text);
			}
		}

		public TextColumnViewModel(AdditionalColumnType additionalColumnType, Employee employee, AdditionalColumn additionalColumn = null)
		{
			AdditionalColumnType = additionalColumnType;
			AdditionalColumn = additionalColumn ?? new AdditionalColumn
			{
				AdditionalColumnType = AdditionalColumnType,
				EmployeeUID = employee.UID,
				TextData = string.Empty
			};
		}
	}
}
