using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Infrastructure.Common.Services;
using Infrastructure.Events;
using StrazhAPI.Models.Layouts;
using Infrastructure.Client.Layout.ViewModels;
using Infrastructure.Common.Services.Layout;
using StrazhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class LayoutPartVerificationViewModel : LayoutPartTitleViewModel
	{
		LayoutPartReferenceProperties _properties;

		public LayoutPartVerificationViewModel(LayoutPartReferenceProperties properties)
		{
			Title = "Верификация";
			IconSource = LayoutPartDescription.IconPath + "BLevels.png";
			_properties = properties ?? new LayoutPartReferenceProperties();
			UpdateLayoutPartInternal(SKDManager.Devices.FirstOrDefault(x => x.UID == _properties.ReferenceUID));

			ServiceFactoryBase.Events.GetEvent<ControllerDeletedEvent>().Subscribe((deviceID) =>
			{
				// Если был удален контроллер, на считыватель которого мы не ссылались
				if (_properties.ReferenceUID == Guid.Empty || SKDManager.Devices.Any(x => x.UID == _properties.ReferenceUID))
					return;
				
				// Если был удален контроллер, на считыватель которого мы ссылались
				Logger.Info(String.Format("Окно верификации '{0}'. Открепление от считывателя ввиду удаления контроллера GUID='{1}'",
					Title,
					deviceID));
				_properties.ReferenceUID = Guid.Empty;
				UpdateLayoutPartInternal(null);
			});
		}

		private void UpdateLayoutPartInternal(SKDDevice device)
		{
			if (device == null)
			{
				UpdateLayoutPart("Устройство не указано");
				return;
			}
			UpdateLayoutPart(new DeviceViewModel(device).NameAndAddress);
		}

		public override ILayoutProperties Properties
		{
			get { return _properties; }
		}
		public override IEnumerable<LayoutPartPropertyPageViewModel> PropertyPages
		{
			get
			{
				yield return new LayoutPartPropertyVerificationPageViewModel(this);
			}
		}

		string _filterTitle;
		public string FilterTitle
		{
			get { return _filterTitle; }
			set
			{
				_filterTitle = value;
				OnPropertyChanged(() => FilterTitle);
			}
		}

		public void UpdateLayoutPart(string name)
		{
			FilterTitle = name;
		}
	}
}