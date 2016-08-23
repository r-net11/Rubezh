using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DepartmentParamsApplyableToEmployeeViewModel : BaseViewModel
	{
		#region <Свойства и поля>

		private bool _showApplyToEmployeeSettings;
		private bool _isEmployee;
		private bool _needApplyAccessTemplateToEmployee;
		private bool _needApplyScheduleToEmployee;

		/// <summary>
		/// Показывать или нет настройки, применяемые для пользователя
		/// </summary>
		public bool ShowApplyToEmployeeSettings
		{
			get { return _showApplyToEmployeeSettings; }
			set
			{
				_showApplyToEmployeeSettings = value;
				OnPropertyChanged(() => ShowApplyToEmployeeSettings);
			}
		}

		/// <summary>
		/// Для кого (сотрудник или посетитель) было вызвано данное окно:
		/// true, если окно было вызвано для сотрудника
		/// false - в противном случае
		/// </summary>
		public bool IsEmployee
		{
			get { return _isEmployee; }
			set
			{
				_isEmployee = value;
				OnPropertyChanged(() => IsEmployee);
			}
		}

		/// <summary>
		/// Состояния чекбокса "Применить шаблоно доступа к сотруднику/посетителю"
		/// </summary>
		public bool NeedApplyAccessTemplateToEmployee
		{
			get { return _needApplyAccessTemplateToEmployee; }
			set
			{
				_needApplyAccessTemplateToEmployee = value;
				OnPropertyChanged(() => NeedApplyAccessTemplateToEmployee);
			}
		}

		/// <summary>
		/// Состояния чекбокса "Применить график работы к сотруднику"
		/// </summary>
		public bool NeedApplyScheduleToEmployee
		{
			get { return _needApplyScheduleToEmployee; }
			set
			{
				_needApplyScheduleToEmployee = value;
				OnPropertyChanged(() => NeedApplyScheduleToEmployee);
			}
		}

		#endregion </Свойства и поля>
	}
}