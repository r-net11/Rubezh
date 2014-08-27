using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class MPTDetailsViewModel : SaveCancelDialogViewModel
	{
		public XMPT MPT { get; set; }

		public MPTDetailsViewModel(XMPT mpt = null)
		{
			if (mpt == null)
			{
				Title = "Создание нового МПТ";

				MPT = new XMPT()
				{
					Name = "Новый МПТ",
					No = 1,
				};
				if (XManager.MPTs.Count != 0)
					MPT.No = (ushort)(XManager.MPTs.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства МПТ: {0}", mpt.Name);
				MPT = mpt;
			}
			CopyProperties();
		}

		void CopyProperties()
		{
			No = MPT.No;
			Name = MPT.Name;
			Description = MPT.Description;
		}

		ushort _no;
		public ushort No
		{
			get { return _no; }
			set
			{
				_no = value;
				OnPropertyChanged(() => No);
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

		protected override bool Save()
		{
			if (MPT.No != No && XManager.MPTs.Any(x => x.No == No))
			{
				MessageBoxService.Show("МПТ с таким номером уже существует");
				return false;
			}

			MPT.No = No;
			MPT.Name = Name;
			MPT.Description = Description;
			return base.Save();
		}

		protected override bool CanSave()
		{
			return true;
		}
	}
}