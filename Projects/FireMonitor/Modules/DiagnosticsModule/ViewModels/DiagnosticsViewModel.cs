using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace DiagnosticsModule.ViewModels
{
	[Serializable]
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			TurnOnOffRMCommand = new RelayCommand(OnTurnOnOffRM);
			CheckHaspCommand = new RelayCommand(OnCheckHasp);
			TestCommand = new RelayCommand(OnTest);
			SKDDataCommand = new RelayCommand(OnSKDData);
		}

		public void StopThreads()
		{
			IsThreadStoping = true;
		}
		bool IsThreadStoping = false;

		string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged(() => Text);
			}
		}

		public RelayCommand TurnOnOffRMCommand { get; private set; }
		void OnTurnOnOffRM()
		{
			var rmDevice = GKManager.Devices.FirstOrDefault(x => x.DriverType == GKDriverType.RM_1 && x.ShleifNo == 3 && x.IntAddress == 1);
			var flag = false;

			var thread = new Thread(new ThreadStart(() =>
			{
				while (true)
				{
					if (IsThreadStoping)
						return;
					Thread.Sleep(TimeSpan.FromMilliseconds(3000));
					flag = !flag;

					ApplicationService.Invoke(() =>
					{
						if (flag)
							Watcher.SendControlCommand(rmDevice, GKStateBit.TurnOn_InManual, "");
						else
							Watcher.SendControlCommand(rmDevice, GKStateBit.TurnOff_InManual, "");
					});
				}
			}));
			thread.Name = "Diagnostics";
			thread.IsBackground = true;
			thread.Start();
		}

		public RelayCommand CheckHaspCommand { get; private set; }
		void OnCheckHasp()
		{
			var thread = new Thread(new ThreadStart(() =>
			{
				while (true)
				{
					ApplicationService.Invoke(() =>
					{
						var hasLicense = LicenseHelper.CheckLicense(false);
					});
					Thread.Sleep(TimeSpan.FromMilliseconds(3000));
				}
			}));
			thread.Name = "Diagnostics";
			thread.IsBackground = true;
			thread.Start();
		}

		public RelayCommand TestCommand { get; private set; }
		void OnTest()
		{
			FiresecManager.FiresecService.Test("");
			return;

			var thread = new Thread(new ThreadStart(() =>
			{
				throw new Exception("TestCommand");
			}));
			thread.Name = "Diagnostics";
			thread.IsBackground = true;
			thread.Start();
		}

		public RelayCommand SKDDataCommand { get; private set; }
		void OnSKDData()
		{
			for (int i = 0; i < 10; i++)
			{
				var org = new OrganisationDetails { Name = "Организация " + i };
				OrganisationHelper.Save(org, true);
				var posUIDs = new List<Guid>();
				for (int j = 0; j < 10; j++)
				{
					var pos = new Position { Name = "Должность " + i + j, OrganisationUID = org.UID };
					PositionHelper.Save(pos, true);
					posUIDs.Add(pos.UID);
				}
				for (int j = 0; j < 10; j++)
				{
					var dept = new Department { Name = "Отдел " + i + j + "0", OrganisationUID = org.UID };
					DepartmentHelper.Save(dept, true);
					for (int k = 0; k < 10; k++)
					{
						var empl = new Employee
						{
							LastName = "Фамилия " + i + j + k + "0",
							FirstName = "Имя " + i + j + k + "0",
							SecondName = "Отчество " + i + j + k + "0",
							Department = DepartmentHelper.GetSingleShort(dept.UID),
							Position = PositionHelper.GetSingleShort(posUIDs.FirstOrDefault()),
							OrganisationUID = org.UID
						};
						EmployeeHelper.Save(empl, true);
					}
					var dept2 = new Department { Name = "Отдел " + i + j + "1", OrganisationUID = org.UID, ParentDepartmentUID = dept.UID };
					DepartmentHelper.Save(dept2, true);
					for (int k = 0; k < 10; k++)
					{
						var empl = new Employee
						{
							LastName = "Фамилия " + i + j + k + "1",
							FirstName = "Имя " + i + j + k + "1",
							SecondName = "Отчество " + i + j + k + "1",
							Department = DepartmentHelper.GetSingleShort(dept2.UID),
							Position = PositionHelper.GetSingleShort(posUIDs.LastOrDefault()),
							OrganisationUID = org.UID
						};
						EmployeeHelper.Save(empl, true);
					}
				}
			}
		}
	}
}