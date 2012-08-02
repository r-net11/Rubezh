using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class DirectionDetailsViewModel : SaveCancelDialogViewModel
    {
        public XDirection XDirection;

		public DirectionDetailsViewModel(XDirection xDirection = null)
        {
			if (xDirection == null)
            {
                Title = "Создание новоого направления";

				XDirection = new XDirection()
                {
                    Name = "Новое направление",
                    No = 1
                };
				if (XManager.DeviceConfiguration.Directions.Count != 0)
					XDirection.No = (short)(XManager.DeviceConfiguration.Directions.Select(x => x.No).Max() + 1);
            }
            else
            {
				Title = string.Format("Свойства направления: {0}", xDirection.PresentationName);
				XDirection = xDirection;
            }
            CopyProperties();
        }

        void CopyProperties()
        {
			Name = XDirection.Name;
			No = XDirection.No;
			Description = XDirection.Description;
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

        short _no;
        public short No
        {
            get { return _no; }
            set
            {
                _no = value;
                OnPropertyChanged("No");
            }
        }

        string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

		protected override bool Save()
		{
			if (XDirection.No != No && XManager.DeviceConfiguration.Directions.Any(x => x.No == No))
            {
                MessageBoxService.Show("Направление с таким номером уже существует");
                return false;
            }

			XDirection.Name = Name;
			XDirection.No = No;
			XDirection.Description = Description;
			return base.Save();
		}
    }
}