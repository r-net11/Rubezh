using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.Models
{
	public class Script : BaseViewModel
	{
		private int _id;
		private bool _isEnabled;
		private string _name;
		private string _description;

		public int Id
		{
			get { return _id; }
			set
			{
				if (_id == value) return;
				_id = value;
				OnPropertyChanged(() => Id);
			}
		}

		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				if (_isEnabled == value) return;
				_isEnabled = value;
				OnPropertyChanged(() => IsEnabled);
			}
		}

		public string Name
		{
			get { return _name; }
			set
			{
				if(string.Equals(_name, value)) return;
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		public string Description
		{
			get { return _description; }
			set
			{
				if (string.Equals(_description, value)) return;
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		public Script(StrazhAPI.Integration.OPC.Script script)
		{
			if (script == null) return;

			Id = script.Id;
			Name = script.Name;
			Description = script.Description;
			IsEnabled = script.IsEnabled;
		}

		public StrazhAPI.Integration.OPC.Script ToDTO()
		{
			return new StrazhAPI.Integration.OPC.Script
			{
				Id = Id,
				Description = Description,
				IsEnabled = IsEnabled,
				Name = Name
			};
		}
	}
}
