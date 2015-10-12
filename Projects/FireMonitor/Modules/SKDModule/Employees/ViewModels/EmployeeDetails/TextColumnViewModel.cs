using RubezhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class TextColumnViewModel: BaseViewModel
	{
		Employee Employee;
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
			Employee = employee;
			AdditionalColumn = additionalColumn != null ? additionalColumn : 
				new AdditionalColumn { AdditionalColumnTypeUID = AdditionalColumnType.UID, ColumnName = AdditionalColumnType.Name, EmployeeUID = Employee.UID, TextData = "" };
		}
	}
}
