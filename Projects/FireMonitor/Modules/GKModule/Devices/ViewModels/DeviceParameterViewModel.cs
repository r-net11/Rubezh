using System;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Common.Windows.Windows;

namespace GKModule.ViewModels
{
	public class DeviceParameterViewModel : BaseViewModel
	{
		public DeviceViewModel DeviceViewModel { get; private set; }
		public GKDevice Device { get; private set; }

		public DeviceParameterViewModel(GKDevice device)
		{
			Device = device;
			DeviceViewModel = new DeviceViewModel(device);

			AUParameterValues = new ObservableCollection<MeasureParameterViewModel>();
			foreach (var measureParameter in device.Driver.MeasureParameters.Where(x => !x.IsNotVisible))
			{
				var measureParameterViewModel = new MeasureParameterViewModel();
				measureParameterViewModel.Device = device;
				measureParameterViewModel.Name = measureParameter.Name;
				measureParameterViewModel.DriverParameter = measureParameter;
				AUParameterValues.Add(measureParameterViewModel);
			}

			device.State.MeasureParametersChanged += new Action(OnMeasureParametersChanged);
		}

		void OnMeasureParametersChanged()
		{
			if (Device.Driver.MeasureParameters.Any(x => x.Name == "Задымленность, дБ/м"))
				Smokiness = " - ";
			if (Device.Driver.MeasureParameters.Any(x => x.Name == "Температура, C"))
				Temperature = " - ";
			if (Device.Driver.MeasureParameters.Any(x => x.Name == "Запыленность, дБ/м"))
				Dustinness = " - ";
			if (Device.Driver.MeasureParameters.Any(x => x.Name == "Сопротивление, Ом"))
				Resistance = " - ";

			foreach (var measureParameterValue in Device.State.XMeasureParameterValues)
			{
				var measureParameterViewModel = AUParameterValues.FirstOrDefault(x => x.Name == measureParameterValue.Name);
				if (measureParameterViewModel != null)
				{
					measureParameterViewModel.StringValue = measureParameterValue.StringValue;
				}

				switch (measureParameterValue.Name)
				{
					case "Задымленность, дБ/м":
						Smokiness = measureParameterValue.StringValue;
						break;

					case "Температура, C":
						Temperature = measureParameterValue.StringValue;
						break;

					case "Запыленность, дБ/м":
						Dustinness = measureParameterValue.StringValue;
						break;

					case "Сопротивление, Ом":
						Resistance = measureParameterValue.StringValue;
						break;
				}
			}

			OnPropertyChanged(() => Temperature);
			OnPropertyChanged(() => Smokiness);
			OnPropertyChanged(() => Dustinness);
			OnPropertyChanged(() => Resistance);
		}

		public string Temperature { get; private set; }
		public string Smokiness { get; private set; }
		public string Dustinness { get; private set; }
		public string Resistance { get; private set; }

		ObservableCollection<MeasureParameterViewModel> _auParameterValues;
		public ObservableCollection<MeasureParameterViewModel> AUParameterValues
		{
			get { return _auParameterValues; }
			set
			{
				_auParameterValues = value;
				OnPropertyChanged(() => AUParameterValues);
			}
		}
	}
}