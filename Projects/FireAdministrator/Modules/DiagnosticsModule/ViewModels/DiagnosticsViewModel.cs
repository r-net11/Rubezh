using System.Linq;
using System.Text;
using DiagnosticsModule.Views;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Firesec.Imitator;

namespace DiagnosticsModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
        public DiagnosticsViewModel()
		{
            ShowDriversCommand = new RelayCommand(OnShowDrivers);
            ShowXDriversCommand = new RelayCommand(OnShowXDrivers);
            ShowTreeCommand = new RelayCommand(OnShowTree);
            Test1Command = new RelayCommand(OnTest1);
            Test2Command = new RelayCommand(OnTest2);
            JournalTestCommand = new RelayCommand(OnJournalTest);
		}

        string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        public RelayCommand ShowDriversCommand { get; private set; }
        void OnShowDrivers()
        {
            var driversView = new DriversView();
            driversView.ShowDialog();
        }

        public RelayCommand ShowXDriversCommand { get; private set; }
        void OnShowXDrivers()
        {
            var driversView = new XDriversView();
            driversView.ShowDialog();
        }

        public RelayCommand JournalTestCommand { get; private set; }
        void OnJournalTest()
        {
            var JournalTestViewModel = new JournalTestViewModel();
            DialogService.ShowModalWindow(JournalTestViewModel);
        }

        public RelayCommand ShowTreeCommand { get; private set; }
        void OnShowTree()
        {
            var devicesTreeViewModel = new DevicesTreeViewModel();
            DialogService.ShowModalWindow(devicesTreeViewModel);
        }

        public RelayCommand Test1Command { get; private set; }
        void OnTest1()
        {
            var stringBuilder = new StringBuilder();

            var Rm1Driver = FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.RM_1);
            foreach (var state in Rm1Driver.States)
            {
                stringBuilder.AppendLine("РМ-1: " + state.Id + " - " + state.Code + " - " + state.Name);
            }

            foreach (var driver in FiresecManager.Drivers)
            {
                foreach (var state in driver.States)
                {
                    if (state.AffectParent)
                    {
                        stringBuilder.AppendLine("AffectParent - " + driver.Name + " - " + state.Name);
                    }
                }
            }

            foreach (var driver in FiresecManager.Drivers)
            {
                foreach (var state in driver.States)
                {
                    if (state.IsManualReset)
                    {
                        stringBuilder.AppendLine("IsManualReset - " + driver.Name + " - " + state.Name);
                    }
                }
            }

            foreach (var driver in FiresecManager.Drivers)
            {
                foreach (var state in driver.States)
                {
                    if (state.CanResetOnPanel)
                    {
                        stringBuilder.AppendLine("CanResetOnPanel - " + driver.Name + " - " + state.Name);
                    }
                }
            }

            foreach (var driver in FiresecManager.Drivers)
            {
                foreach (var state in driver.States)
                {
                    if (state.IsAutomatic)
                    {
                        stringBuilder.AppendLine("Automatic " + driver.Name + " - " + state.Name);
                    }
                    //if (state.IsAutomatic && state.Name.Contains("AutoOff"))
                    //{
                    //    stringBuilder.AppendLine("Automatic AutoOff - " + driver.Name + " - " + state.Name);
                    //}
                    //if (state.IsAutomatic && state.Name.Contains("Auto_Off"))
                    //{
                    //    stringBuilder.AppendLine("Automatic Auto_Off - " + driver.Name + " - " + state.Name);
                    //}
                }
            }

            foreach (var driver in FiresecManager.Drivers)
            {
                foreach (var state in driver.States)
                {
                    if (state.IsAutomatic)
                    {
                        stringBuilder.AppendLine("Automatic - " + driver.Name + " - " + state.Name + " - " + state.Code);
                    }
                }
            }
            Text = stringBuilder.ToString();
        }

        public RelayCommand Test2Command { get; private set; }
        void OnTest2()
        {
			ImitatorService.Show();
        }
    }
}