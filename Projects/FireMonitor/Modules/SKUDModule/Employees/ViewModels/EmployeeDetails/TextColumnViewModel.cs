using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class TextColumnViewModel: BaseViewModel
	{
		public AdditionalColumn AdditionalColumn { get; private set; }
		public string Name { get; private set; }
		public string Text
		{
			get { return AdditionalColumn.TextData; }
			set
			{
				AdditionalColumn.TextData = value;
				OnPropertyChanged(() => Text);
			}
		}

		public TextColumnViewModel(AdditionalColumn additionalColumn)
		{
			AdditionalColumn = additionalColumn;
			Name = AdditionalColumn.AdditionalColumnType.Name;
		}
	}
}
