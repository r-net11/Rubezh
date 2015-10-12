using System;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class DayIntervalDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<DayInterval>
	{
		RubezhAPI.SKD.Organisation Organisation;
		public DayInterval Model { get; private set; }
		bool _isNew;

		public bool Initialize(Organisation organisation, DayInterval model, ViewPartViewModel parentViewModel)
		{
			Organisation = organisation;
			_isNew = model == null;
			if (_isNew)
			{
				Title = "Новый дневной график";
				model = new DayInterval()
				{
					Name = "Дневной график",
					OrganisationUID = organisation.UID,
				};
				model.DayIntervalParts.Add(new DayIntervalPart() { BeginTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(18, 0, 0), DayIntervalUID = model.UID, Number = 1 });
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

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name) && Name != "Всегда" && Name != "Никогда";
		}
		protected override bool Save()
		{
			Model.Name = Name;
			Model.Description = Description;
			Model.SlideTime = ConstantSlideTime;
			if (DetailsValidateHelper.Validate(Model) && DayIntervalHelper.Save(Model, _isNew))
			{
				ServiceFactory.Events.GetEvent<EditDayIntervalEvent>().Publish(Model.UID);
				return true;
			}
			else
				return false;
		}
	}
}