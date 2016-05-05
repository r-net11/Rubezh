using System;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DayIntervalDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<DayInterval>
	{
		#region Fields
		bool _isNew;
		#endregion

		#region Properties
		public DayInterval Model { get; private set; }

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

		TimeSpan _constantSlideTime;
		public TimeSpan ConstantSlideTime
		{
			get { return _constantSlideTime; }
			set
			{
				_constantSlideTime = value;
				OnPropertyChanged(() => ConstantSlideTime);
			}
		}
		#endregion

		#region Commands
		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name) && Name != "Всегда" && Name != "Никогда";
		}
		protected override bool Save()
		{
			Model.Name = Name;
			Model.Description = Description;
			Model.SlideTime = ConstantSlideTime;

			return DayIntervalHelper.Save(Model, _isNew);
		}
		#endregion

		#region Methods
		public bool Initialize(Organisation organisation, DayInterval model, ViewPartViewModel parentViewModel)
		{
			_isNew = model == null;
			if (_isNew)
			{
				Title = "Новый дневной график";
				model = new DayInterval
				{
					Name = "Дневной график",
					OrganisationUID = organisation.UID,
				};
				model.DayIntervalParts.Add(new DayIntervalPart { BeginTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(18, 0, 0), DayIntervalUID = model.UID });
				Model = model;
			}
			else
			{
				Title = "Редактирование дневного графика";
				Model = DayIntervalHelper.GetSingle(model.UID);
			}
			Name = Model.Name;
			Description = Model.Description;
			ConstantSlideTime = Model.SlideTime;
			return true;
		}
		#endregion
	}
}