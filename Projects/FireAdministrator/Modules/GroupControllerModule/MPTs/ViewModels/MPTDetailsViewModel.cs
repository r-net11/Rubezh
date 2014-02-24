using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

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
					Name = "Новый МПТ"
				};
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
			Name = MPT.Name;
        }

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}


		protected override bool Save()
		{
			MPT.Name = Name;
			return base.Save();
		}

        protected override bool CanSave()
        {
			return true;
        }
	}
}