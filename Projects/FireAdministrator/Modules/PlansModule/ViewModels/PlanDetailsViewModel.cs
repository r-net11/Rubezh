using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace PlansModule.ViewModels
{
    public class PlanDetailsViewModel : SaveCancelDialogContent
    {
        bool _isNew;
        public Plan Plan { get; private set; }

        public PlanDetailsViewModel(Plan plan = null)
        {
            if (plan != null)
            {
                Title = "Редактировать план";
                Plan = plan;
                _isNew = false;
            }
            else
            {
                Title = "Создать план";
                Plan = new Plan()
                {
                    Name = "Новый план",
                    Width = 400,
                    Height = 400
                };
                _isNew = true;
            }

            CopyProperties();
        }

        void CopyProperties()
        {
            Name = Plan.Name;
            Width = Plan.Width;
            Height = Plan.Height;
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

        double _height;
        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged("Height");
            }
        }

        double _width;
        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged("Width");
            }
        }

        protected override bool CanSave()
        {
            return !string.IsNullOrEmpty(Name);
        }

        protected override void Save(ref bool cancel)
        {
            Plan.Name = Name;
            Plan.Height = Height;
            Plan.Width = Width;
            if (_isNew)
            {
                FiresecManager.PlansConfiguration.Plans.Add(Plan);
            }
        }
    }
}