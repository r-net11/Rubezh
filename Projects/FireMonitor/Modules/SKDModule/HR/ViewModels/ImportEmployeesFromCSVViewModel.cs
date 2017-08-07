using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace SKDModule.ViewModels
{
	public class ImportEmployeesFromCSVViewModel : DialogViewModel
	{
		public ImportEmployeesFromCSVViewModel()
		{
			SelectFileCommand = new RelayCommand(SelectFile);
			ImportCommand = new RelayCommand(Import, CanImport);
			AvailableOrganisations = new ObservableCollection<Organisation>(OrganisationHelper.Get(new OrganisationFilter()));
			Title = "Добавление сотрудников из файла";
			StatusText = "Выберите файл";
		}

		public RelayCommand SelectFileCommand { get; set; }
		void SelectFile()
		{
			var openFileDialog = new Microsoft.Win32.OpenFileDialog();
			var showDialog = openFileDialog.ShowDialog();
			if (showDialog != null && showDialog.Value)
			{
				Employees = ReadCSVFile(openFileDialog.FileName);
				StatusText = openFileDialog.FileName;
				OnPropertyChanged(() => StatusText);
			}
		}

		public bool ShowErrors { get; set; }

		public string StatusText { get; set; }
		DataTable ReadCSVFile(string pathToCsvFile)
		{
			DataTable dt = new DataTable("Employees");

			DataColumn fio = new DataColumn("FIO", typeof(String));
			DataColumn lastName = new DataColumn("LastName", typeof(String));
			DataColumn firstName = new DataColumn("FirstName", typeof(String));
			DataColumn fatherName = new DataColumn("FatherName", typeof(String));
			DataColumn id = new DataColumn("ID", typeof(int));
			DataColumn studentID = new DataColumn("StudentID", typeof(string));
			DataColumn department = new DataColumn("Department", typeof(String));
			DataColumn year = new DataColumn("Year", typeof(string));

			dt.Columns.AddRange(new DataColumn[] {
				fio,
				lastName,
				firstName,
				fatherName,
				id,
				studentID,
				department,
				year
			});

			DataRow dr = null;
			string[] employee = null;
			try
			{
				string[] employees = File.ReadAllLines(pathToCsvFile, Encoding.GetEncoding(1251));
				for (int i = 1; i < employees.Length; i++)
				{
					if (!String.IsNullOrEmpty(employees[i]))
					{
						employee = employees[i].Split(';');

						dr = dt.NewRow();

						dr["ID"] = int.Parse(employee[0].Replace(" ", string.Empty));
						dr["FIO"] = employee[1];
						dr["LastName"] = employee[2];
						dr["FirstName"] = employee[3];
						dr["FatherName"] = employee[4];
						dr["Department"] = employee[5];
						var tmpYear = 0;
						int.TryParse(employee[6], out tmpYear);
						dr["Year"] = tmpYear.ToString();
						// dr["StudentID"] = int.Parse(employee[8].Replace(" ", string.Empty).Replace("-", string.Empty));
						dr["StudentID"] = employee[8];

						dt.Rows.Add(dr);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			return dt;
		}

		public Organisation SelectedOrganisation { get; set; }

		public ObservableCollection<Organisation> AvailableOrganisations { get; set; }

		public DataTable Employees { get; set; }

		public RelayCommand ImportCommand { get; set; }
		void Import()
		{
			var organisationUID = SelectedOrganisation.UID;
			var addedEmployees = new List<Employee>();
			var existingEmployees = EmployeeHelper.GetShortByOrganisation(organisationUID);
			if (Employees != null)
			{
				var additionalIDColumn = AdditionalColumnTypeHelper.GetByOrganisation(organisationUID).FirstOrDefault(x => x.Name == "ID сотрудника/студента");
				if (additionalIDColumn == null)
				{
					additionalIDColumn = new AdditionalColumnType { Name = "ID сотрудника/студента", OrganisationUID = organisationUID, IsInGrid = true};
					AdditionalColumnTypeHelper.Save(additionalIDColumn, true);
				}

				var additionalFIOColumn = AdditionalColumnTypeHelper.GetByOrganisation(organisationUID).FirstOrDefault(x => x.Name == "ФИО");
				if (additionalFIOColumn == null)
				{
					additionalFIOColumn = new AdditionalColumnType { Name = "ФИО", OrganisationUID = organisationUID, IsInGrid = true };
					AdditionalColumnTypeHelper.Save(additionalFIOColumn, true);
				}

				var additionalStudentIDColumn = AdditionalColumnTypeHelper.GetByOrganisation(organisationUID).FirstOrDefault(x => x.Name == "Номер студенческого билета");
				if (additionalStudentIDColumn == null)
				{
					additionalStudentIDColumn = new AdditionalColumnType { Name = "Номер студенческого билета", OrganisationUID = organisationUID, IsInGrid = true };
					AdditionalColumnTypeHelper.Save(additionalStudentIDColumn, true);
				}


				foreach (DataRow employee in Employees.Rows)
				{
					var currentDepartment = DepartmentHelper.Get(new DepartmentFilter()).FirstOrDefault(x => x.Name == employee["Department"].ToString());
					if (currentDepartment == null)
					{
						var tmp = new Department { Name = employee["Department"].ToString(), OrganisationUID = organisationUID };
						if (ShowErrors)
						{
							DepartmentHelper.Save(tmp, true);
						}
						else
						{
							DepartmentHelper.Save(tmp, true, true);
						}
						currentDepartment = DepartmentHelper.Get(new DepartmentFilter()).FirstOrDefault(x => x.Name == employee["Department"].ToString());
					}
					var currentPosition = PositionHelper.Get(new PositionFilter()).FirstOrDefault(x => x.Name == employee["Year"].ToString());
					if (currentPosition == null)
					{
						var tmp = new Position { Name = employee["Year"].ToString(), OrganisationUID = organisationUID };
						PositionHelper.Save(tmp, true);
						currentPosition = PositionHelper.Get(new PositionFilter()).FirstOrDefault(x => x.Name == employee["Year"].ToString());
					}

					var newEmployee = new Employee
					{
						FirstName = employee["FirstName"].ToString(),
						LastName = employee["LastName"].ToString(),
						SecondName = employee["FatherName"].ToString(),
						Department = currentDepartment,
						OrganisationUID = organisationUID,
						Position = currentPosition,
					};

					newEmployee.AdditionalColumns.Add(new AdditionalColumn { AdditionalColumnType = additionalIDColumn, TextData = employee["ID"].ToString(), EmployeeUID = newEmployee.UID });
					newEmployee.AdditionalColumns.Add(new AdditionalColumn { AdditionalColumnType = additionalStudentIDColumn, TextData = employee["StudentID"].ToString(), EmployeeUID = newEmployee.UID });
					newEmployee.AdditionalColumns.Add(new AdditionalColumn { AdditionalColumnType = additionalFIOColumn, TextData = employee["FIO"].ToString(), EmployeeUID = newEmployee.UID });
					
					if (addedEmployees.FirstOrDefault(x => x.Name == newEmployee.Name) == null && existingEmployees.FirstOrDefault(x => x.Name == newEmployee.Name) == null)
					{
						EmployeeHelper.Save(newEmployee, true);
						addedEmployees.Add(newEmployee);
					}
				}
			}
			else
			{
				MessageBoxService.Show("Нет данных для импорта.");
			}
			MessageBoxService.Show("Добавлено " + addedEmployees.Count + " записей.");
			Close();
		}

		bool CanImport()
		{
			return Employees != null && SelectedOrganisation != null;
		}
	}
}
