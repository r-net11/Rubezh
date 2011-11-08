using FiresecAPI.Models;
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
                Plan = new Plan();
                _isNew = true;
            }

            CopyProperties();
        }

        void CopyProperties()
        {
            Caption = Plan.Caption;
            Description = Plan.Description;
            Width = Plan.Width;
            Height = Plan.Height;
        }

        string _caption;
        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                OnPropertyChanged("Caption");
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
            return !string.IsNullOrEmpty(Caption);
        }

        protected override void Save(ref bool cancel)
        {
            Plan.Caption = Caption;
            Plan.Description = Description;
            Plan.Height = Height;
            Plan.Width = Width;
        }
    }
}