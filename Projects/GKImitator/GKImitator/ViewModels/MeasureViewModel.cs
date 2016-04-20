using RubezhAPI.GK;
using Infrastructure.Common.Windows.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace GKImitator.ViewModels
{
	public class MeasureViewModel : SaveCancelDialogViewModel
	{
		public GKDevice Device { get; private set; }
		public List<MeasureParameterValue> MeasureParameterValues { get; private set; }

		public MeasureViewModel(GKDevice device, List<MeasureParameterValue> measureParameterValues)
		{
			Title = "Измерения";
			Device = device;
			MeasureParameterValues = measureParameterValues;

			MeasureParameters = new ObservableCollection<MeasureParameterViewModel>();
			foreach (var measureParameter in device.Driver.MeasureParameters)
			{
				var measureParameterValue = measureParameterValues.FirstOrDefault(x => x.MeasureParameter.No == measureParameter.No);
				if (measureParameterValue == null)
				{
					measureParameterValue = new MeasureParameterValue();
					measureParameterValue.MeasureParameter = measureParameter;
				}
				var measureParameterViewModel = new MeasureParameterViewModel(measureParameterValue);
				MeasureParameters.Add(measureParameterViewModel);
			}
		}

		public ObservableCollection<MeasureParameterViewModel> MeasureParameters { get; private set; }

		protected override bool Save()
		{
			MeasureParameterValues = new List<MeasureParameterValue>();
			foreach (var measureParameterViewModel in MeasureParameters)
			{
				MeasureParameterValues.Add(new MeasureParameterValue() { MeasureParameter = measureParameterViewModel.MeasureParameter, Value = measureParameterViewModel._value });
			}
			return base.Save();
		}
	}

	public class MeasureParameterViewModel : BaseViewModel
	{
		public GKMeasureParameter MeasureParameter { get; private set; }

		public MeasureParameterViewModel(MeasureParameterValue measureParameterValue)
		{
			MeasureParameter = measureParameterValue.MeasureParameter;
			_value = measureParameterValue.Value;
		}

		public ushort _value;
		public string Value
		{
			get
			{
				double result = _value;
				if (MeasureParameter.Multiplier.HasValue && MeasureParameter.Multiplier != 0)
					result /= MeasureParameter.Multiplier.Value;
				return result.ToString();
			}
			set
			{
				double doubleValue = -1;
				if (double.TryParse(value.Replace(".", ","),
									NumberStyles.Number,
									CultureInfo.CreateSpecificCulture("ru-RU"),
									out doubleValue))
				{
					if (MeasureParameter.Multiplier.HasValue && MeasureParameter.Multiplier != 0)
						doubleValue *= MeasureParameter.Multiplier.Value;
					doubleValue = Math.Min(ushort.MaxValue, doubleValue);
					_value = (ushort)doubleValue;
				}
				OnPropertyChanged(() => Value);
			}
		}
	}

	public class MeasureParameterValue
	{
		public GKMeasureParameter MeasureParameter { get; set; }
		public ushort Value { get; set; }
	}
}