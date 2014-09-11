using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using System.Collections.ObjectModel;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class CodeSelectionViewModel : SaveCancelDialogViewModel
	{
		public CodeSelectionViewModel(XCode selectedCode)
		{
			Title = "Выбор кода";
			Codes = new ObservableCollection<XCode>();
			foreach (var code in XManager.DeviceConfiguration.Codes)
			{
				Codes.Add(code);
			}
			if (selectedCode != null)
				SelectedCode = Codes.FirstOrDefault(x => x.UID == selectedCode.UID);
		}

		public ObservableCollection<XCode> Codes { get; private set; }

		XCode _selectedCode;
		public XCode SelectedCode
		{
			get { return _selectedCode; }
			set
			{
				_selectedCode = value;
				OnPropertyChanged(() => SelectedCode);
			}
		}

		protected override bool Save()
		{
			return base.Save();
		}
	}
}