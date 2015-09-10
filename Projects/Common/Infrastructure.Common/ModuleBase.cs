using FiresecAPI;
using Infrastructure.Common.Navigation;
using System.Collections.Generic;
using System.ComponentModel;

namespace Infrastructure.Common
{
	public abstract class ModuleBase : IModule
	{
		public ModuleBase()
		{
			RegisterResource();
		}

		#region IModule Members

		public string Name
		{
			get { return ModuleType.ToDescription(); }
		}

		public abstract ModuleType ModuleType { get; }

		public virtual int Order
		{
			get { return 10; }
		}

		public virtual void RegisterResource()
		{
			ResourceService resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
		}

		public virtual bool BeforeInitialize(bool firstTime)
		{
			return true;
		}

		public virtual void AfterInitialize()
		{
		}

		public abstract void CreateViewModels();

		public abstract void Initialize();

		public abstract IEnumerable<NavigationItem> CreateNavigation();

		#endregion IModule Members

		#region IDisposable Members

		public virtual void Dispose()
		{
		}

		#endregion IDisposable Members
	}

	public enum ModuleType
	{
		[DescriptionAttribute("Администратор")]
		Administrator,

		[DescriptionAttribute("Автоматизация")]
		Automation,

		[DescriptionAttribute("Устройства, Зоны")]
		Devices,

		[DescriptionAttribute("Фильтры журнала событий")]
		Filters,

		[DescriptionAttribute("Конфигуратор макетов ОЗ")]
		Layout,

		[DescriptionAttribute("Библиотека устройств")]
		Library,

		[DescriptionAttribute("Уведомления")]
		Notification,

		[DescriptionAttribute("Графические планы")]
		Plans,

		[DescriptionAttribute("Права доступа")]
		Security,

		[DescriptionAttribute("Настройки")]
		Settings,

		[DescriptionAttribute("СКД")]
		SKD,

		[DescriptionAttribute("Страж")]
		Strazh,

		[DescriptionAttribute("Звуки")]
		Sounds,

		[DescriptionAttribute("Видео")]
		Video,

		[DescriptionAttribute("Cостояния")]
		Alarm,

		[DescriptionAttribute("Журнал событий и Архив")]
		Journal,

		[DescriptionAttribute("Отчёты")]
		Reports,

		[DescriptionAttribute("Монитор")]
		Monitor,
	}
}