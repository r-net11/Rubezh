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
		public CodeSelectionViewModel(GKCode selectedCode)
		{
			Title = "Выбор кода";
			Codes = new ObservableCollection<GKCode>();
			foreach (var code in GKManager.DeviceConfiguration.Codes)
			{
				Codes.Add(code);
			}
			if (selectedCode != null)
				SelectedCode = Codes.FirstOrDefault(x => x.UID == selectedCode.UID);
		}

		public ObservableCollection<GKCode> Codes { get; private set; }

		GKCode _selectedCode;
		public GKCode SelectedCode
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