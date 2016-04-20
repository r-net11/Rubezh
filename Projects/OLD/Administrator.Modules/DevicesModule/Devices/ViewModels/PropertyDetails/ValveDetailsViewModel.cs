using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class ValveDetailsViewModel : SaveCancelDialogViewModel
	{
		Device Device;

		public ValveDetailsViewModel(Device device)
		{
			Title = "Параметры устройства: ШУЗ";
			Actions = new List<string>() { "0", "1" };
			Device = device;

			var actionProperty = Device.Properties.FirstOrDefault(x => x.Name == "Action");
			if ((actionProperty == null) || (actionProperty.Value == null))
				SelectiedAction = "0";
			else
				SelectiedAction = actionProperty.Value;
		}

		public List<string> Actions { get; private set; }

		string _selectiedAction;
		public string SelectiedAction
		{
			get { return _selectiedAction; }
			set
			{
				_selectiedAction = value;
				OnPropertyChanged(() => SelectiedAction);
			}
		}

		protected override bool Save()
		{
			var actionProperty = Device.Properties.FirstOrDefault(x => x.Name == "Action");
			if (actionProperty == null)
			{
				actionProperty = new Property() { Name = "Action" };
				Device.Properties.Add(actionProperty);
			}
			actionProperty.Value = SelectiedAction;
			return base.Save();
		}
	}
}